using Cocona.Builder;
using Supervisor.Application.Services;
using Supervisor.Cli.Application.Common;

namespace Supervisor.Cli.Application.Commands;

public class StatusCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("status", async (RuntimeProcessManager runtimeManager) =>
            {
                string status;
                using (StartSpinner("Checking runtime status..."))
                {
                    var ctSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    var runtimeIsRunning = await runtimeManager.IsRunningAsync(ctSource.Token);

                    status = runtimeIsRunning ? "[RUNNING]" : "[NOT RUNNING]";
                }

                Info("Runtime status:", false);
                Write($" {status}", status == "[RUNNING]" ? ConsoleColor.Green : ConsoleColor.Red);
            })
            .WithDescription("Get the current status of services");
    }
}