using Microsoft.Extensions.Logging;
using Supervisor.Cli;
using Supervisor.Cli.Common;

Console.OutputEncoding = Encoding.Unicode;

var builder = CoconaApp.CreateBuilder()
    .InstallServices();

builder.Logging.SetMinimumLevel(LogLevel.Error);

builder
    .Build()
    .MapCommands()
    .Run();