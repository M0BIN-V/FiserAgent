using Cocona.Builder;

namespace Supervisor.Cli.Application.Common ;

public interface ICommand
{
    public void Map(ICoconaCommandsBuilder builder);
}