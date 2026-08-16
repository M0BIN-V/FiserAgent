using Fiser.Supervisor;
using Fiser.Supervisor.Commands;

var path = Path.Combine();

Console.OutputEncoding = Encoding.Unicode;
Console.Clear();

var app = CoconaApp
    .CreateBuilder()
    .InstallServices()
    .Build();

app.AddCommands<MainCommands>();

app.Run();