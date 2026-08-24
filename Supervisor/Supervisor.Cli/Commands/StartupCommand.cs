// using Supervisor.Application.Features.Interfaces.StartInterfaces;
// using Supervisor.Application.Features.Runtime.GetLatestVersion;
// using Supervisor.Application.Features.Runtime.GetRuntimeStatus;
// using Supervisor.Application.Features.Runtime.InstallRuntime;
// using Supervisor.Application.Features.Runtime.StartRuntime;
//
// namespace Supervisor.Cli.Commands;
//
// public class StartupCommand : ICommand
// {
//     public void Map(ICoconaCommandsBuilder builder)
//     {
//         builder.AddCommand(Handler)
//             .WithDescription("main command for handling runtime operations");
//     }
//
//     private static async Task Handler(
//         [FromService] StartInterfacesHandler startInterfacesHandler,
//         [FromService] GetRuntimeStatusHandler runtimeStatusHandler,
//         [FromService] StartRuntimeHandler startHandler,
//         [FromService] InstallRuntimeHandler installHandler,
//         [FromService] GetLatestVersionHandler getLatestVersionHandler)
//     {
//         var runtimeStatus = await runtimeStatusHandler.HandleAsync(new GetRuntimeStatusRequest());
//
//         var latestVersionResult = await getLatestVersionHandler.HandleAsync(new GetLatestVersionRequest());
//
//         var latestVersion = latestVersionResult.Version;
//
//         if (!runtimeStatus.Installed)
//         {
//             var progressBar = Progress("installing runtime...");
//             await installHandler.HandleAsync(new InstallRuntimeRequest(latestVersion, progressBar),
//                 CancellationToken.None);
//             Success("runtime is installed");
//         }
//
//         if (runtimeStatus.Version < latestVersion)
//         {
//             var progressBar = Progress("updating runtime...");
//             await installHandler.HandleAsync(new InstallRuntimeRequest(latestVersion, progressBar),
//                 CancellationToken.None);
//             Success("runtime is updated");
//         }
//
//         runtimeStatus = await runtimeStatusHandler.HandleAsync(new GetRuntimeStatusRequest());
//
//         Uri endpoint;
//
//         if (runtimeStatus.IsRunning)
//             endpoint = runtimeStatus.Endpoint!;
//         else
//             using (StartSpinner("running runtime"))
//             {
//                 using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
//                 var result = await startHandler.HandleAsync(new StartRuntimeRequest(), timeout.Token);
//                 endpoint = result.AsT0;
//             }
//
//         Success("runtime started");
//
//         using (StartSpinner("starting interfaces"))
//         {
//             await startInterfacesHandler.HandleAsync(
//                 new StartInterfacesRequest(endpoint), CancellationToken.None);
//         }
//     }
// }