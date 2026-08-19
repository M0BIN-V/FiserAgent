using Cocona.Builder;
using Fiser.Supervisor.Common;
using Fiser.Supervisor.Helpers.Tui;
using Fiser.Supervisor.Options;
using Fiser.Supervisor.Services;
using Microsoft.Extensions.Options;

namespace Fiser.Supervisor.Commands;

public class ShoutdownCommand : ICommand
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
        Message.Info("finding runtime process...");
        if (!await runtimeProcessManager.IsRunningAsync(CancellationToken.None))
        {
            Message.Warning("runtime is not running.");
            return;
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await runtimeProcessManager.ShutdownAsync(timeout.Token);

        Message.Success("runtime shutdown completed.");
    }
}