using System.IO.Pipes;
using System.Text;
using Microsoft.Extensions.Logging;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Services.Process;

namespace Supervisor.Infra.Services;

public class PipeClient(ILogger<PipeClient> logger) : IPipeClient, IAsyncDisposable
{
    private NamedPipeClientStream? _pipe;
    private StreamReader? _reader;
    private StreamWriter? _writer;

    private bool IsConnected => _pipe?.IsConnected == true;

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

        logger.LogDebug($"Connecting to pipe :{pipeName}");

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
        logger.LogDebug("sending pipe command");
        if (_writer is null || !IsConnected) throw new InvalidOperationException("Pipe is not connected.");

        await _writer.WriteLineAsync(command.AsMemory(), cancellationToken);
    }

    public async Task<string?> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("command received from pipe");
        if (_reader is null || !IsConnected) throw new InvalidOperationException("Pipe is not connected.");

        return await _reader.ReadLineAsync(cancellationToken);
    }
}