namespace Supervisor.Cli.Commands.Interfaces;

public class ChatInterface
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required Version Version { get; set; }
    public required Version RequiredRuntimeVersion { get; set; }
}

public interface IChatInterfaceManager
{
    IEnumerable<ChatInterface> GetInstalledInterfaces();
}

public class InterfacesCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("interfaces", () => { });
    }
}