namespace Supervisor.Application.Common.Abstractions;

public abstract class Handler<TRequest>
{
    public abstract Task Handle(TRequest request, CancellationToken ct);
}

public abstract class Handler<TRequest, TResponse>
{
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}