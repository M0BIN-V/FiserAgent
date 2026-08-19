using Fiser.Supervisor;
using Fiser.Supervisor.Common;

Console.OutputEncoding = Encoding.Unicode;

CoconaApp.CreateBuilder()
    .InstallServices()
    .Build()
    .MapCommands()
    .Run();