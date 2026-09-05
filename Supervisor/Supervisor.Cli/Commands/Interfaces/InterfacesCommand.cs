using Supervisor.Application.Features.Interfaces.GetList;
using Supervisor.Application.Features.Interfaces.Install;
using Supervisor.Cli.Helpers.Tui;

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

                    var result = await StartSpinnerAsync($"installing {selected}", () =>
                    {
                        return installHandler.HandleAsync(new InstallInterfaceRequest(
                            selected,
                            getResult.Interfaces.Single(i => i.UniqueName == selected).Version
                        ));
                    });

                    result.Switch(
                        version => Success($"{selected} v{version} installed"),
                        error => Error(error.Message),
                        error => Error(error.Message));
                })
                .WithDescription("installs interface");
        });
    }
}