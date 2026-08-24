using Supervisor.Application.Features.Interfaces.GetList;
using Supervisor.Application.Features.Interfaces.Install;

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

                    foreach (var iface in response.Interfaces)
                        Console.WriteLine($"- {iface.Name} ({iface.UniqueName})");
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

                    InstallInterfaceResponse result;

                    using (StartSpinner($"installing {selected}"))
                    {
                        result =
                            await installHandler.HandleAsync(new InstallInterfaceRequest(
                                selected,
                                getResult.Interfaces.Single(i => i.UniqueName == selected).version
                            ));
                    }

                    result.Switch(
                        version => Success($"{selected} v{version} installed"),
                        error => Error(error.Message),
                        error => Error(error.Message));
                })
                .WithDescription("installs interface");
        });
    }
}