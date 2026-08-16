using Microsoft.Extensions.Hosting;
using TeleFrame.ApplicationBuilder;
using TeleFrame.Middlewares;
using TeleFrame.UpdateHandlers.MessageHandlers.CommandHandlers;

var builder = new TelegramBotBuilder(args);

builder.Services.AddUpdateLogging();

var app = builder.Build();

app.UseUpdateLogging();

app.MapCommand("/start", () => "hi this is fiser");

app.Run();