using Supervisor.Application.Features.Runtime.GetLatestVersion;
using Supervisor.Application.Features.Runtime.GetRuntimeStatus;
using Supervisor.Application.Features.Runtime.InstallRuntime;
using Supervisor.Application.Features.Runtime.StartRuntime;
using Supervisor.Application.Features.Shutdown;
using Supervisor.Application.Services;

namespace Supervisor.Cli.Application.Commands;

public class MainCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand(Handler)
            .WithDescription("main command for handling runtime operations");
    }

    private static async Task Handler(
        [FromService] ShutdownHandler shutdownHandler,
        [FromService] GetRuntimeStatusHandler runtimeStatusHandler,
        [FromService] StartRuntimeHandler startHandler,
        [FromService] RuntimeProcessManager runtimeProcessManager,
        [FromService] InstallRuntimeHandler installHandler,
        GetLatestVersionHandler getLatestVersionHandler)
    {
        var (installed, installedRuntimeVersion, _) = await runtimeStatusHandler
            .HandleAsync(new GetRuntimeStatusRequest());

        var latestVersionResult = await getLatestVersionHandler.HandleAsync(new GetLatestVersionRequest());

        var latestVersion = latestVersionResult.Version;
        
        if (!installed)
        {
            var progressBar = Progress("installing runtime...");
            await installHandler.HandleAsync(new InstallRuntimeRequest(latestVersion, progressBar),
                CancellationToken.None);
            Success("runtime is installed");
        }

        if (installedRuntimeVersion < latestVersion)
        {
            var progressBar = Progress("updating runtime...");
            await installHandler.HandleAsync(new InstallRuntimeRequest(latestVersion, progressBar),
                CancellationToken.None);
            Success("runtime is updated");
        }

        var (_, _, isRunning) = await runtimeStatusHandler.HandleAsync(new GetRuntimeStatusRequest());

        if (!isRunning)
            using (StartSpinner("running runtime"))
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await startHandler.HandleAsync(new StartRuntimeRequest(), timeout.Token);
            }

        Success("runtime started");
    }
}