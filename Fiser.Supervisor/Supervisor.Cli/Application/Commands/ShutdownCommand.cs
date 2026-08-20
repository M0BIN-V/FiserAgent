using Cocona.Builder;
using Fiser.Supervisor.Cli.Services;
using Microsoft.Extensions.Options;
using Supervisor.Cli.Application.Common;
using Supervisor.Cli.Options;
using Supervisor.Cli.Services;

namespace Supervisor.Cli.Application.Commands;

public class ShutdownCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("shutdown", Handler)
            .WithDescription("shutdown runtime process");
    }

    private static async Task Handler(
        [FromService] RuntimeProcessManager runtimeProcessManager,
        [FromService] IOptions<RuntimeOptions> runtimeOptions)
    {
        Info("finding runtime process...");
        if (!await runtimeProcessManager.IsRunningAsync(CancellationToken.None))
        {
            Warning("runtime is not running.");
            return;
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await runtimeProcessManager.ShutdownAsync(timeout.Token);

        Success("runtime shutdown completed.");
    }
}