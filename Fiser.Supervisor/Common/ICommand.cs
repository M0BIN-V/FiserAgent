using Cocona.Builder;

namespace Fiser.Supervisor.Common;

public interface ICommand
{
    public void Map(ICoconaCommandsBuilder builder);
}