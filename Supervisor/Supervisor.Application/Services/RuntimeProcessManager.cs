using System.Diagnostics;
using System.Net;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Options;
using Supervisor.Application.Services.ProcessProfile;

namespace Supervisor.Application.Services;

public sealed record RuntimeEndpoint(Uri Address);

public class RuntimeProcessProfile : ProcessProfile.ProcessProfile
{
    public string Url { get; set; } = null!;
}

public sealed class RuntimeProcessManager(
    IOptions<RuntimeOptions> options,
    ProfileService<RuntimeProcessProfile> profileService,
    PipeClient pipeClient,
    ILogger<RuntimeProcessManager> logger,
    ILogger<ProcessManager<RuntimeProcessProfile>> baseLogger,
    HttpClient httpClient) :
    ProcessManager<RuntimeProcessProfile>(baseLogger, pipeClient, profileService)
{
    private readonly Channel<string> _output = Channel.CreateUnbounded<string>();
    public readonly Channel<string> Errors = Channel.CreateUnbounded<string>();
    public readonly Channel<string> Output = Channel.CreateUnbounded<string>();

    public override async Task<bool> IsRunningHealthyAsync(CancellationToken ct)
    {
        return await base.IsRunningHealthyAsync(ct) && await RespondsHealthyAsync(ct);
    }

    public async Task<bool> RespondsHealthyAsync(CancellationToken ct)
    {
        var profile = await ProfileService.GetProfileAsync(ct);

        var baseUrl = new Uri(profile.Url);

        var aliveUrl = new Uri(baseUrl, "alive");

        var response = await httpClient.GetAsync(aliveUrl, ct);

        return response.StatusCode is HttpStatusCode.OK;
    }

    public async Task StartAsync(Dictionary<string, string> environmentVariables, CancellationToken ct = default)
    {
        await StartProcess(options.Value.FilePath, environmentVariables, ct);

        var endpoint = await WaitForEndpointAsync(ct);

        var profile = await ProfileService.GetProfileAsync(ct);

        profile.Url = endpoint.Address.ToString();

        await ProfileService.UpdateProfileAsync(profile, ct);
    }

    protected override void OnError(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is null) return;
        Errors.Writer.TryWrite(e.Data);

        logger.LogError("Runtime error: {Error}", e.Data);
    }

    protected override void OnOutput(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is null) return;
        _output.Writer.TryWrite(e.Data);

        logger.LogDebug("Runtime output: {Output}", e.Data);
    }


    private async Task<RuntimeEndpoint> WaitForEndpointAsync(CancellationToken cancellationToken)
    {
        await foreach (var line in _output.Reader.ReadAllAsync(cancellationToken))
        {
            if (TryParseEndpoint(line, out var endpoint)) return endpoint;

            Output.Writer.TryWrite(line);

            if (Process!.HasExited) throw new InvalidOperationException("Runtime exited before becoming ready.");
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
}