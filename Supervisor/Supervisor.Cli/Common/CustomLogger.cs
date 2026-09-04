using Microsoft.Extensions.Logging;

namespace Supervisor.Cli.Common;

public sealed class CustomLogger(string categoryName) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Information;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);


        Console.CursorLeft = 0;
        Disable($"[{DateTimeOffset.Now:HH:mm:ss}]", false);
        Disable($"[{logLevel}]" , false);
        Disable($"[{categoryName.Split('.').Last()}]");
        Disable(message);
        Disable("----------------------------------");

        if (exception is not null) Console.WriteLine(exception);
    }
}

public sealed class CustomLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new CustomLogger(categoryName);
    }

    public void Dispose()
    {
    }
}