using System.Diagnostics;
using System.Net;
using System.Threading.Channels;
using Supervisor.Application.Common.Options;

namespace Supervisor.Application.Services;

public sealed record RuntimeEndpoint(Uri Address);

public sealed class RuntimeProcessManager(
    IRuntimeProcessProfileService profileService,
    RuntimePipeClient pipeClient,
    HttpClient httpClient) : IDisposable
{
    private readonly Channel<string> _output = Channel.CreateUnbounded<string>();
    public readonly Channel<string> Errors = Channel.CreateUnbounded<string>();
    public readonly Channel<string> Output = Channel.CreateUnbounded<string>();

    private Process? _process;

    public void Dispose()
    {
        _process?.Dispose();
    }

    public static bool IsProcessRunning(int processId, string expectedName)
    {
        try
        {
            using var process = Process.GetProcessById(processId);

            return !process.HasExited && string.Equals(
                process.ProcessName,
                expectedName,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public async Task<bool> IsRunningAsync(CancellationToken ct)
    {
        if (!profileService.ProfileExists()) return false;

        var runtimeProfile = await profileService.GetProfileAsync(ct);

        return IsProcessRunning(runtimeProfile.ProcessId, "Fiser.Runtime") &&
               await RespondsHealthyAsync(ct);
    }

    public async Task<bool> RespondsHealthyAsync(CancellationToken ct)
    {
        var profile = await profileService.GetProfileAsync(ct);

        var baseUrl = new Uri(profile.Url);

        var aliveUrl = new Uri(baseUrl, "alive");

        var response = await httpClient.GetAsync(aliveUrl, ct);

        return response.StatusCode is HttpStatusCode.OK;
    }

    public async Task StartAsync(string filePath, CancellationToken ct = default)
    {
        if (await IsRunningAsync(ct)) throw new InvalidOperationException("Runtime is already running.");

        var pipeName = $"fiser-runtime-{Guid.NewGuid():N}";

        var startInfo = new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.Environment["PIPE_NAME"] = pipeName;

        _process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        _process.OutputDataReceived += OnOutput;
        _process.ErrorDataReceived += OnError;

        if (!_process.Start()) throw new InvalidOperationException("Failed to start runtime.");

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        // await pipeClient.ConnectAsync(pipeName, ct);

        var endpoint = await WaitForEndpointAsync(ct);

        var profile = new RuntimeProcessProfile
        {
            Url = endpoint.Address.ToString(),
            ProcessId = _process.Id,
            PipeName = pipeName
        };

        await profileService.UpdateProfileAsync(profile, ct);
    }

    private void OnOutput(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is null) return;

        _output.Writer.TryWrite(e.Data);
    }

    private void OnError(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is null) return;
        Errors.Writer.TryWrite(e.Data);
    }

    private async Task<RuntimeEndpoint> WaitForEndpointAsync(CancellationToken cancellationToken)
    {
        await foreach (var line in _output.Reader.ReadAllAsync(cancellationToken))
        {
            if (TryParseEndpoint(line, out var endpoint)) return endpoint;

            Output.Writer.TryWrite(line);

            if (_process!.HasExited) throw new InvalidOperationException("Runtime exited before becoming ready.");
        }

        throw new InvalidOperationException("Runtime output ended before runtime became ready.");
    }

    private static bool TryParseEndpoint(string line, out RuntimeEndpoint endpoint)
    {
        const string prefix = "Now listening on:";
        line = line.Trim();

        if (!line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            endpoint = null!;
            return false;
        }

        var value = line[prefix.Length..].Trim();

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            endpoint = null!;
            return false;
        }

        endpoint = new RuntimeEndpoint(uri);

        return true;
    }

    public async Task ShutdownAsync(CancellationToken ct)
    {
        var profile = await profileService.GetProfileAsync(ct);

        await pipeClient.ConnectAsync(profile.PipeName, ct);

        await pipeClient.ShutdownAsync(ct);
        await pipeClient.DisposeAsync();
    }
}