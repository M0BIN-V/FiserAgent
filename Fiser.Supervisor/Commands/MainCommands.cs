using Fiser.Supervisor.Services;
using static Fiser.Supervisor.Helpers.Tui.Message;

namespace Fiser.Supervisor.Commands;

public class MainCommands
{
    public async Task Main([FromService] IRuntimeService runtimeService)
    {
        Info("loading runtime...");

        var runtimeInstalled = runtimeService.RunIsTimeInstalled();
        if (!runtimeInstalled)
        {
            var result = Select("no runtime installed. do you want to install it?", ["yes", "no"]);

            if (result is "yes")
            {

                var progressBar = Progress("fetching runtime...");

                await runtimeService.FetchRuntimeAsync(progressBar);
            }
            else return;
        }

        Success("runtime loaded.");
        
        Info("running runtime...");
        
        
    }
}