namespace Supervisor.Application.Common.Contracts;

public interface IPipeClient : IAsyncDisposable
{
    Task ConnectAsync(string pipeName, CancellationToken cancellationToken = default);
    Task SendAsync(string command, CancellationToken cancellationToken = default);
    Task<string?> ReceiveAsync(CancellationToken cancellationToken = default);
}