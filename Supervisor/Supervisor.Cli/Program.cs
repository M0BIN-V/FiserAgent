using Microsoft.Extensions.Logging;
using Supervisor.Cli;

Console.OutputEncoding = Encoding.Unicode;

var logLevel = args.Any(a => a.Equals("--debug", StringComparison.OrdinalIgnoreCase))
    ? LogLevel.Debug
    : LogLevel.Error;

args.Replace("--debug", string.Empty);

var builder = CoconaApp.CreateBuilder(args)
    .InstallServices();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(new CustomLoggerProvider());

builder.Logging.SetMinimumLevel(logLevel);

builder
    .Build()
    .MapCommands()
    .Run();