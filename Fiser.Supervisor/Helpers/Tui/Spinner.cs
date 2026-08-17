namespace Fiser.Supervisor.Helpers.Tui;

public sealed class Spinner : IDisposable
{
    private static readonly char[] Frames =
    [
        '⠋',
        '⠙',
        '⠹',
        '⠸',
        '⠼',
        '⠴',
        '⠦',
        '⠧',
        '⠇',
        '⠏'
    ];

    private readonly CancellationTokenSource _cts;
    private readonly string _message;
    private readonly Task _task;

    public Spinner(string message)
    {
        _message = message;
        _cts = new CancellationTokenSource();

        _task = RunAsync(_cts.Token);
    }

    public void Dispose()
    {
        _cts.Cancel();

        try
        {
            _task.GetAwaiter().GetResult();
        }
        catch
        {
            // ignored
        }

        _cts.Dispose();
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var index = 0;

        Console.CursorVisible = false;
        var currentCursorLeft =  Console.CursorLeft;
        var currentCursorTop = Console.CursorTop;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                
                Console.Write($"\r{Frames[index++ % Frames.Length]} {_message}");

                await Task.Delay(80, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            Console.Write("\r" + new string(' ', _message.Length + 3) + "\r");

            Console.CursorVisible = true;
        }
    }
}