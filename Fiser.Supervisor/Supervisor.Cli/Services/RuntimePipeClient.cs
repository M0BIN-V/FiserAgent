using System.IO.Pipes;

namespace Fiser.Supervisor.Cli.Services;

public sealed class RuntimePipeClient
{
    private NamedPipeClientStream? _pipe;
    private StreamWriter? _writer;

    public bool IsConnected => _pipe?.IsConnected == true;

    public async Task ConnectAsync(string pipeName, CancellationToken cancellationToken = default)
    {
        if (IsConnected) return;

        var pipe = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.Out,
            PipeOptions.Asynchronous);

        await pipe.ConnectAsync(cancellationToken);

        _pipe = pipe;

        _writer = new StreamWriter(pipe, Encoding.UTF8, leaveOpen: true)
        {
            AutoFlush = true
        };
    }

    public async Task SendAsync(string command, CancellationToken cancellationToken = default)
    {
        if (_writer is null || !IsConnected)
            throw new InvalidOperationException("Runtime pipe is not connected.");

        await _writer.WriteLineAsync(command);
    }

    public async Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        await SendAsync("shutdown", cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_writer is not null) await _writer.DisposeAsync();

        if (_pipe is not null) await _pipe.DisposeAsync();

        _writer = null;
        _pipe = null;
    }
}