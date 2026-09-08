using System.Diagnostics;
using System.Threading.Channels;
using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Common.Settings;
using Supervisor.Application.Services.Process.Runtime;

namespace Supervisor.Application.Features.Runtime.StartRuntime;

public class StartRuntimeHandler(
    RuntimeSettings runtimeSettings,
    IRuntimeService runtimeService,
    RuntimeProfileService profileService,
    IProcessManagerFactory managerFactory,
    ILogger<StartRuntimeHandler> logger) :
    Handler<StartRuntimeRequest, StartRuntimeResponse>
{
    private readonly Channel<string> _output = Channel.CreateUnbounded<string>();
            
    public override async Task<StartRuntimeResponse> HandleAsync(StartRuntimeRequest request,
        CancellationToken ct = default)
    {
        if (!runtimeService.RunIsTimeInstalled()) return new RuntimeIsNotInstalledError();

        RuntimeProcessProfile profile;

        if (profileService.ProfileExists()) profile = await profileService.GetProfileAsync(ct);
        else
            profile = new RuntimeProcessProfile
            {
                PipeName = Guid.CreateVersion7().ToString("N"),
                ProcessId = 0,
                ProcessName = null,
                Url = null
            };

        var manager = managerFactory.Create(profile);

        if (await manager.IsRunningHealthyAsync(ct)) return new RuntimeIsAlreadyRunningError();

        var env = new Dictionary<string, string>
        {
            ["OTEL_EXPORTER_OTLP_TRACES_PROTOCOL"] = "grpc",
            ["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:4317"
        };

        var runtimeBinaryPath = runtimeSettings.BinaryPath ??
                                throw new Exception("Runtime binary path is missing");

        var process = await manager.StartProcess(runtimeBinaryPath,
            env,
            onOutput: OnOutput,
            onError: OnError,
            ct: ct);

        profile.Url = await WaitForEndpointAsync(process, ct);

        if (!await manager.IsRunningHealthyAsync(CancellationToken.None))
            throw new Exception("Runtime did not respond healthy after starting.");

        await profileService.UpdateProfileAsync(profile, ct);


        return new StartRuntimeResponse(new Uri(profile.Url));
    }

    private async Task<string> WaitForEndpointAsync(Process process, CancellationToken cancellationToken)
    {
        await foreach (var line in _output.Reader.ReadAllAsync(cancellationToken))
        {
            if (TryParseEndpoint(line, out var endpoint)) return endpoint;

            if (process.HasExited) throw new InvalidOperationException("Runtime exited before becoming ready.");
        }

        throw new InvalidOperationException("Runtime output ended before runtime became ready.");
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null) return;
        _output.Writer.TryWrite(e.Data);
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        logger.LogError("[RUNTIME ERROR] " + e.Data);
    }

    private static bool TryParseEndpoint(string line, out string endpoint)
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

        endpoint = uri.ToString();

        return true;
    }
}