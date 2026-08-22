using Supervisor.Application.Features.Interfaces.GetList;

namespace Supervisor.Cli.Commands.Interfaces;

public class InterfacesCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddSubCommand("interfaces", sub =>
        {
            sub.AddCommand("list", async ([FromService] GetInterfacesListHanlder handler) =>
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
        });
    }
}