using Supervisor.Application.Features.Runtime.InstallRuntime;
using Supervisor.Application.Features.Runtime.Shutdown;
using Supervisor.Application.Features.Runtime.StartRuntime;
using Supervisor.Application.Features.Shutdown;

namespace Supervisor.Cli.Commands.Runtime;

public class RuntimeCommands : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddSubCommand("runtime", sub =>
        {
            sub.AddCommand("start", async (
                    [FromService] StartRuntimeHandler handler) =>
                {
                    var result = await handler
                        .HandleAsync(new StartRuntimeRequest(), CancellationToken.None);

                    result.Switch(
                        uri => Success("runtime started"),
                        notInstalled =>
                        {
                            Error("runtime is not installed");
                            SuggestCommand("runtime install", "install runtime");
                        },
                        alreadyRunning => Success("runtime is running"));
                })
                .WithDescription("starts fiser runtime process");

            sub.AddCommand("install", async (
                    [Option('v')] string? version,
                    [FromService] InstallRuntimeHandler handler) =>
                {
                    //TODO validate version

                    var ct = CancellationToken.None;

                    InstallRuntimeResponse result;

                    var progress = Progress("installing runtime");

                    result = await handler.HandleAsync(
                        new InstallRuntimeRequest(progress: progress), ct);

                    Success($"runtime v{result.installedVersion} installed.");
                })
                .WithDescription("installs runtime");

            sub.AddCommand("stop", async (
                    [FromService] ShutdownRuntimeHandler runtimeHandler) =>
                {
                    var runtimeResponse = await StartSpinnerAsync("shutting down...", () =>
                        runtimeHandler.HandleAsync(new ShutdownRuntimeRequest(), CancellationToken.None));

                    if (runtimeResponse.runtimeWasNotRunning)
                        Warning("runtime is not running.");
                    else
                        Success("runtime shutdown completed.");
                })
                .WithDescription("shutdown runtime process");
        });
    }
}