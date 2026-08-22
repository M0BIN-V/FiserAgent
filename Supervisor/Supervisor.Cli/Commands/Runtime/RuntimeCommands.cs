using Supervisor.Application.Features.Runtime.InstallRuntime;
using Supervisor.Application.Features.Runtime.StartRuntime;

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
                var ct = CancellationToken.None;

                InstallRuntimeResponse result;

                using (StartSpinner("installing runtime"))
                {
                    result = await handler.HandleAsync(
                        new InstallRuntimeRequest(), ct);
                }

                Success($"runtime v{result.installedVersion} installed.");
            });
        });
    }
}