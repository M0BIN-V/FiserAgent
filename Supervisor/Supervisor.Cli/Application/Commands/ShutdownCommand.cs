using Cocona.Builder;
using Supervisor.Application.Features.Shutdown;
using Supervisor.Cli.Application.Common;

namespace Supervisor.Cli.Application.Commands;

public class ShutdownCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("shutdown", async ([FromService] ShutdownHandler handler) =>
            {
                ShutdownResponse response;

                using (StartSpinner("shutting down..."))
                {
                    response = await handler.HandleAsync();
                }

                if (response.runtimeWasNotRunning)
                    Warning("runtime is not running.");
                else
                    Success("runtime shutdown completed.");
            })
            .WithDescription("shutdown runtime process");
    }
}