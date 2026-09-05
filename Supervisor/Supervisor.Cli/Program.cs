using Microsoft.Extensions.Logging;
using Spectre.Console;
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

var app = builder.Build();

app.UseFilter(async (ctx, next) =>
{
    try
    {
        return await next(ctx);
    }
    catch (CommandExitedException)
    {
        throw;
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteException(ex,
            ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        return 1;
    }
});

app.MapCommands()
    .Run();