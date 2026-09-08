using Supervisor.Application.Features.Interfaces.Configure;
using Supervisor.Application.Features.Interfaces.GetInstalledInterfaces;
using Supervisor.Application.Features.Interfaces.GetList;
using Supervisor.Application.Features.Interfaces.Install;
using Supervisor.Application.Features.Interfaces.Start;
using Supervisor.Cli.Helpers.Tui.Components;

namespace Supervisor.Cli.Commands.Interfaces;

public class InterfacesCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddSubCommand("interfaces", sub =>
        {
            sub.AddCommand("list", async ([FromService] GetInterfacesListHandler handler) =>
                {
                    var response = await handler.HandleAsync(new GetInterfacesRequest(), CancellationToken.None);


                    if (response.Interfaces.Count == 0)
                    {
                        Warning("No interfaces found.");
                        return;
                    }

                    var table = new Table()
                        .AddColumns("Unique name", "Name", "Version");

                    foreach (var @interface in response.Interfaces)
                        table.AddRow(@interface.UniqueName, @interface.Name, @interface.Version.ToString());

                    table.Print();
                })
                .WithDescription("list all agent interfaces");

            sub.AddCommand("install", async (
                    [FromService] GetInterfacesListHandler getHandler,
                    [FromService] InstallInterfaceHandler installHandler) =>
                {
                    var getResult = await getHandler
                        .HandleAsync(new GetInterfacesRequest(), CancellationToken.None);

                    var interfaceNames = getResult.Interfaces
                        .Select(i => i.UniqueName)
                        .ToList();

                    var selected = Select("select interface to install", interfaceNames);
                    var selectedVersion = getResult.Interfaces
                        .Single(i => i.UniqueName == selected).Version;

                    var result = await StartSpinnerAsync($"installing {selected}", () =>
                        installHandler.HandleAsync(new InstallInterfaceRequest(selected, selectedVersion),
                            CancellationToken.None));

                    result.Switch(
                        version => Success($"{selected} v{version} installed"),
                        error => Error(error.Message),
                        error => Error(error.Message));
                })
                .WithDescription("installs interface");

            sub.AddCommand("configure", async (
                [FromService] GetInstalledInterfacesHandler installedHandler,
                [FromService] ConfigureInterfaceHandler handler) =>
            {
                var selected = await GetInstalledInterfaceName(installedHandler);
                var result = await handler.HandleAsync(new ConfigureInterfaceRequest(selected));
            });

            sub.AddCommand("start", async (
                [FromService] StartInterfacesHandler startHandler,
                [FromService] GetInstalledInterfacesHandler installedHandler) =>
            {
                var selectedUniqueName = await GetInstalledInterfaceName(installedHandler);

                var startRequest = new StartInterfacesRequest(selectedUniqueName);
                var startResult = await startHandler.HandleAsync(startRequest);

                startResult.Switch(
                    started => Success("interface started"),
                    notFound => Error(notFound.Message),
                    alreadyRunning => Success("already running"),
                    runtimeIsNotRunning => Error(runtimeIsNotRunning.Message));
            });
        });
    }

    private async Task<string> GetInstalledInterfaceName(GetInstalledInterfacesHandler handler)
    {
        var installedResult = await handler.HandleAsync(new GetInstalledInterfacesRequest());

        var uniqueNames = installedResult.Interfaces
            .Select(i => i.UniqueName)
            .ToList();

        return Select("select interface:", uniqueNames);
    }
}