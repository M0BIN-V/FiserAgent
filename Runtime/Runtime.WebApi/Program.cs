using System.Text.Json.Serialization;
using DiServiceInstaller;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Runtime.WebApi;
using Runtime.WebApi.Services;
using Runtime.WebApi.Tools;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.InstallServices(typeof(Program).Assembly);

builder.Services.AddHostedService<RuntimePipeService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

var systemPrompt = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "system.md"));


app.MapPost("completion", async (CompletionRequest request, IChatClient chatClient) =>
{
    var options = new ChatClientAgentOptions
    {
        Name = "fiser",
        ChatOptions = new ChatOptions
        {
            Instructions = systemPrompt
        }
    };
    options.ChatOptions.InstallTools();
    var agent = chatClient.AsAIAgent(options);
    Store.Session ??= await agent.CreateSessionAsync();

    async IAsyncEnumerable<CompletionResponse> GetEvents()
    {
        await foreach (var update in agent.RunStreamingAsync(request.Text, Store.Session))
        foreach (var content in update.Contents)
        {
            var result = content switch
            {
                TextContent t => string.IsNullOrEmpty(t.Text) ? null : CompletionResponse.CreateText(t.Text),
                FunctionCallContent f => CompletionResponse.CreateToolCall(f.Name, f.Name),
                FunctionResultContent fr => CompletionResponse.CreateToolResult(fr.Result),
                _ => null
            };

            if (result is not null) yield return result;
        }
    }

    return TypedResults.ServerSentEvents(GetEvents());
});

app.Run();