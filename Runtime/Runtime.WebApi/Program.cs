using System.Text.Json.Serialization;
using DiServiceInstaller;
using Microsoft.AspNetCore.Mvc;
using Runtime.Application;
using Runtime.Application.Features.CompleteChat;
using Runtime.WebApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.InstallApplicationServices();

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

app.MapPost("completion", async ([FromBody] CompleteChatRequest request, [FromServices] CompleteChatHandler handler) =>
{
    var result = await handler.HandleAsync(request);

    return TypedResults.ServerSentEvents(result);
});

app.Run();