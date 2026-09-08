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

app.MapPost("completion", async (string message, IChatClient chatClient) =>
{
    var agent = chatClient.AsAIAgent(
        "you are a helpful assistant that answers questions in a concise and clear manner called fiser.",
        "fiser");

    var response = await agent.RunAsync(message);

    return TypedResults.Ok(response.Text);
});

app.Run();