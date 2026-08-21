using Supervisor.Application.Features.Shutdown;

namespace Supervisor.Cli.Commands;

public class ShutdownCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("shutdown", async ([FromService] ShutdownHandler handler) =>
            {
                ShutdownResponse response;

                using (StartSpinner("shutting down..."))
                {
                    response = await handler.HandleAsync(new ShutdownRequest(), CancellationToken.None);
                }

                if (response.runtimeWasNotRunning)
                    Warning("runtime is not running.");
                else
                    Success("runtime shutdown completed.");
            })
            .WithDescription("shutdown runtime process");
    }
}