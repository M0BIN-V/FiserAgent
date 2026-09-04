using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace Supervisor.Application.Services;

public sealed class PipeClient : IAsyncDisposable
{
    private NamedPipeClientStream? _pipe;
    private StreamReader? _reader;
    private StreamWriter? _writer;

    public bool IsConnected => _pipe?.IsConnected == true;

    public async ValueTask DisposeAsync()
    {
        _reader?.Dispose();

        if (_writer is not null) await _writer.DisposeAsync();

        if (_pipe is not null) await _pipe.DisposeAsync();

        _reader = null;
        _writer = null;
        _pipe = null;
    }

    public async Task ConnectAsync(string pipeName, CancellationToken cancellationToken = default)
    {
        if (IsConnected) return;

        var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

        await pipe.ConnectAsync(cancellationToken);

        _pipe = pipe;

        _writer = new StreamWriter(pipe, Encoding.UTF8, leaveOpen: true)
        {
            AutoFlush = true
        };

        _reader = new StreamReader(pipe, Encoding.UTF8, leaveOpen: true);
    }

    public async Task SendAsync(string command, CancellationToken cancellationToken = default)
    {
        if (_writer is null || !IsConnected) throw new InvalidOperationException("Pipe is not connected.");

        await _writer.WriteLineAsync(command.AsMemory(), cancellationToken );
    }

    public async Task<string?> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        if (_reader is null || !IsConnected) throw new InvalidOperationException("Pipe is not connected.");

        return await _reader.ReadLineAsync(cancellationToken);
    }


    public async Task<TimeSpan> PingAsync(
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        await SendAsync("ping", cancellationToken);

        var response = await ReceiveAsync(cancellationToken);

        stopwatch.Stop();

        if (response != "pong") throw new InvalidOperationException($"Unexpected ping response: {response}");

        return stopwatch.Elapsed;
    }

    public async Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        await SendAsync("shutdown", cancellationToken);
    }
}