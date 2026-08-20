using Cocona.Builder;
using Fiser.Supervisor.Common;
using Fiser.Supervisor.Helpers.Tui;
using Fiser.Supervisor.Services;

namespace Fiser.Supervisor.Commands;

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

                Message.Info("Runtime status:", false);
                Message.Write($" {status}", status == "[RUNNING]" ? ConsoleColor.Green : ConsoleColor.Red);
            })
            .WithDescription("Get the current status of services");
    }
}