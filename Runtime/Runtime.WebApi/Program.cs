using DiServiceInstaller;
using Microsoft.Extensions.AI;
using Runtime.WebApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.InstallServices(typeof(Program).Assembly);

builder.Services.AddHostedService<RuntimePipeService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

var systemPrompt = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory,"system.md"));


app.MapPost("completion", async (string message, IChatClient chatClient) =>
{
    var agent = chatClient.AsAIAgent(
        systemPrompt,
        "fiser");

    var result = await agent.RunAsync(message);

    var response = new CompletionResponse(
        result.Text,
#pragma warning disable MEAI001
        result.Usage?.InputTokenCount ?? 0,
        result.Usage?.OutputTokenCount ?? 0);
#pragma warning restore MEAI001

    return TypedResults.Ok(response);
});

app.Run();

public record CompletionResponse(
    string Text,
    long inputToken,
    long outputToken);