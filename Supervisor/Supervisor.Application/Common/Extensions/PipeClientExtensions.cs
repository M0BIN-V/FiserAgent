using System.Diagnostics;
using Supervisor.Application.Services.Process;

namespace Supervisor.Application.Common.Extensions;

public static class PipeClientExtensions
{
    extension(IPipeClient client)
    {
        public async Task<TimeSpan> PingAsync(CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            await client.SendAsync("ping", cancellationToken);

            var response = await client.ReceiveAsync(cancellationToken);

            stopwatch.Stop();

            if (response != "pong") throw new InvalidOperationException($"Unexpected ping response: {response}");

            return stopwatch.Elapsed;
        }

        public async Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            await client.SendAsync("shutdown", cancellationToken);
        }
    }
}