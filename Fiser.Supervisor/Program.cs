using System.Text;
using Cocona;
using Fiser.Supervisor.Helpers.Tui;
using Fiser.Supervisor.Services;
using Microsoft.Extensions.DependencyInjection;

Console.OutputEncoding = Encoding.Unicode;

var builder = CoconaApp.CreateBuilder();


builder.Services.AddSingleton<IRuntimeManager, DebugRuntimeManager>();

var app = builder.Build();

app.AddCommand(async (IRuntimeManager runtimeManager) =>
{
    Message.Info("validating runtime...");

    var currentRuntimeVersion = runtimeManager.GetCurrentRuntimeVersion();
    var lastRuntimeVersion = runtimeManager.GetLastRuntimeVersion();

    if (currentRuntimeVersion < lastRuntimeVersion)
    {
        Message.Warning(
            $"the latest version of runtime is {lastRuntimeVersion} but installed version is {currentRuntimeVersion}");

        var confirmed = ConsoleUi.Select("do you want to install the latest version of runtime ?", ["yes", "no"]);


        if (confirmed is "yes")
            using (ConsoleUi.StartSpinner("Installing runtime..."))
            {
                await Task.Delay(3000);
                Message.Error("noooooooooo");
            }
    }
});

app.AddCommand("runtime", (IRuntimeManager manager) =>
{
    var version = manager.GetCurrentRuntimeVersion();

    if (version is null)
        Console.WriteLine("Runtime not found!");

    Console.WriteLine($"v{version}");
});

app.Run();

//
// var bar = new ProgressBar();
//
// bar.MaxValue = 100;
//
// bar.Show(20);
// var progress = new Progress<CopyProgress>(p => { Console.WriteLine($"{p.Percentage:F1}%"); });
//
// var source = @"E:\Projects\FiserAgent\Fiser.Supervisor\bin\Debug";
// var destination = @"E:\Projects\FiserAgent\Fiser.Supervisor\bin\Debug2";
//
// await FileHelpers.CopyDirectoryAsync(source, destination, progress);