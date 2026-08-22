namespace Supervisor.Cli.Common;

public interface ICommand
{
    public void Map(ICoconaCommandsBuilder builder);
}