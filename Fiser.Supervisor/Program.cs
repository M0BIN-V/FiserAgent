using Fiser.Supervisor;
using Fiser.Supervisor.Common;

var path = Path.Combine();

Console.OutputEncoding = Encoding.Unicode;
Console.Clear();

CoconaApp.CreateBuilder()
    .InstallServices()
    .Build()
    .MapCommands()
    .Run();