using Supervisor.Cli;
using Supervisor.Cli.Application.Common;

Console.OutputEncoding = Encoding.Unicode;

CoconaApp.CreateBuilder()
    .InstallServices()
    .Build()
    .MapCommands()
    .Run();