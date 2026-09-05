using Supervisor.Application.Features.Runtime.GetRuntimeStatus;

namespace Supervisor.Cli.Commands;

public class StatusCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("status", async (GetRuntimeStatusHandler handler) =>
            {
                var status = await StartSpinnerAsync("Checking runtime status...", () =>
                {
                    var ctSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    return handler.HandleAsync(new GetRuntimeStatusRequest(), ctSource.Token);
                });


                if (!status.Installed)
                {
                    Info("Runtime status:", false);
                    Write(" [NOT INSTALLED]", ConsoleColor.Yellow);
                    return;
                }

                var statusText = status.IsRunning ? "[RUNNING]" : "[NOT RUNNING]";
                Info("Runtime status:", false);
                Write($" {statusText}", status.IsRunning ? ConsoleColor.Green : ConsoleColor.Red);
            })
            .WithDescription("get the current status of services");
    }
}