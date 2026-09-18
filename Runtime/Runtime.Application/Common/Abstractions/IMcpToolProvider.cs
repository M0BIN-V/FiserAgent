using Microsoft.Extensions.AI;

namespace Runtime.Application.Common.Abstractions;

public interface IMcpToolProvider
{

    public Task<IReadOnlyList<AIFunction>> GetToolsAsync(CancellationToken ct = default);
}