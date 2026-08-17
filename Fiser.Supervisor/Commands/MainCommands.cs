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
            else
            {
                return;
            }
        }
        else
        {
            Info("validating runtime version...");
            var currentVersion = await runtimeService.GetRuntimeVersionAsync();
            var latestVersion = await runtimeService.GetLatestRuntimeVersionAsync();

            if (currentVersion < latestVersion)
            {
              Warning($"new runtime version is available: {latestVersion}");
              var result = Select($"do you want to update runtime ? (current version : {currentVersion})", ["yes", "no"]);

              if (result is "yes")
              {
                  Info("updating runtime...");
              }
            }
            else
            {
                Success("runtime is updated");
            }
        }

        Success("runtime loaded.");

        Info("running runtime...");
    }
}