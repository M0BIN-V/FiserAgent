using System.IO.Pipes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Interfaces.Sdk;

public sealed class InterfacePipeService(
    IConfiguration config,
    IHostApplicationLifetime lifetime,
    ILogger<InterfacePipeService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pipeName = config["SUPERVISOR_PIPE_NAME"];

        if (string.IsNullOrWhiteSpace(pipeName))
            throw new InvalidOperationException(
                "SUPERVISOR_PIPE_NAME config is not set.");

        logger.LogInformation(
            "Starting interface pipe server: {PipeName}",
            pipeName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await using var pipe = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);

            logger.LogInformation(
                "Waiting for supervisor connection...");

            try
            {
                await pipe.WaitForConnectionAsync(stoppingToken);

                logger.LogInformation(
                    "Supervisor connected.");

                using var reader = new StreamReader(pipe);
                await using var writer = new StreamWriter(pipe);
                writer.AutoFlush = true;

                while (!stoppingToken.IsCancellationRequested)
                {
                    var command = await reader.ReadLineAsync(stoppingToken);

                    if (command is null)
                    {
                        logger.LogInformation(
                            "Supervisor disconnected.");

                        break;
                    }

                    logger.LogInformation(
                        "Received command: {Command}",
                        command);

                    if (command.Equals(
                            "ping",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteLineAsync("pong");

                        logger.LogDebug("Sent pong.");

                        continue;
                    }

                    if (command.Equals(
                            "shutdown",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogInformation(
                            "Shutdown command received.");

                        lifetime.StopApplication();

                        return;
                    }

                    logger.LogWarning("Unknown command received: {Command}",
                        command);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        logger.LogInformation("Interface pipe server stopped.");
    }
}