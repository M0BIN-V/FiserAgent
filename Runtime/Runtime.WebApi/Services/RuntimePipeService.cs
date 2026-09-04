using System.IO.Pipes;

namespace Runtime.WebApi.Services;

public sealed class RuntimePipeService(
    IConfiguration config,
    IHostApplicationLifetime lifetime,
    ILogger<RuntimePipeService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pipeName = config.GetValue<string>("SUPERVISOR_PIPE_NAME");

        if (string.IsNullOrWhiteSpace(pipeName))
            throw new InvalidOperationException("SUPERVISOR_PIPE_NAME config is not set.");

        logger.LogInformation(
            "Starting runtime pipe server: {PipeName}",
            pipeName);

        await using var pipe = new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut,
            1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous);

        logger.LogInformation("Waiting for supervisor connection...");

        await pipe.WaitForConnectionAsync(stoppingToken);

        logger.LogInformation("Supervisor connected.");

        using var reader = new StreamReader(pipe);
        await using var writer = new StreamWriter(pipe);
        writer.AutoFlush = true;

        while (!stoppingToken.IsCancellationRequested)
        {
            var command = await reader.ReadLineAsync(stoppingToken);

            if (command is null)
                break;

            logger.LogInformation("Received command: {Command}", command);

            if (command.Equals("ping", StringComparison.OrdinalIgnoreCase))
            {
                await writer.WriteLineAsync("pong");

                logger.LogDebug("Sent pong.");

                continue;
            }

            if (command.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Shutdown command received.");

                lifetime.StopApplication();

                break;
            }
        }
    }
}