using System.Text.Json;
using Cocona.Builder;
using Fiser.Supervisor.Common;
using Fiser.Supervisor.Options;
using Fiser.Supervisor.Services;
using Microsoft.Extensions.Options;
using static Fiser.Supervisor.Helpers.Tui.Message;

namespace Fiser.Supervisor.Commands;

public class MainCommand : ICommand
{
    public void Map(ICoconaCommandsBuilder builder)
    {
        builder.AddCommand(Handler)
            .WithDescription("main command for handling runtime operations");
    }

    private static async Task Handler(
        [FromService] IRuntimeService runtimeService,
        [FromService] RuntimeProcessManager runtimeProcessManager,
        [FromService] IRuntimeRegistry registry,
        [FromService] IOptions<RuntimeOptions> runtimeOptions)
    {
        Info("loading runtime...");

        var runtimeInstalled = runtimeService.RunIsTimeInstalled();
        if (!runtimeInstalled)
        {
            Info("runtime not installed.");
            var progressBar = Progress("fetching runtime...");

            await registry.FetchRuntimeAsync(progressBar);
        }
        else
        {
            Info("validating runtime version...");
            var currentVersion = await runtimeService.GetRuntimeVersionAsync();
            var latestVersion = await registry.GetLatestRuntimeVersionAsync();

            if (currentVersion < latestVersion)
            {
                Warning($"new runtime version is available: {latestVersion}");
                var result = Select($"do you want to update runtime ? (current version : {currentVersion})",
                    ["yes", "no"]);

                if (result is "yes") Info("updating runtime...");
                var progressBar = Progress("fetching runtime...");
                await registry.FetchRuntimeAsync(progressBar);
            }
            else
            {
                Success("runtime is updated");
            }
        }

        Success($"runtime v{await runtimeService.GetRuntimeVersionAsync()} loaded.");

        if (!await runtimeProcessManager.IsRunningAsync())
            using (StartSpinner("running runtime"))
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                await runtimeProcessManager.StartAsync(runtimeOptions.Value.FilePath, timeout.Token);
            }

        var endpoint = new RuntimeEndpoint(new Uri(
            JsonSerializer.Deserialize<RuntimeProcessProfile>(
                await File.ReadAllTextAsync(runtimeOptions.Value.ProcessProfile))!.Url));

        if (await runtimeProcessManager.RespondsHealthyAsync())
            Success($"runtime started on {endpoint.Address}");
        else
            Error("runtime failure!");
    }
}