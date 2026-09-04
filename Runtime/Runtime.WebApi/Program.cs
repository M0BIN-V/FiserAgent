using Runtime.WebApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddHostedService<RuntimePipeService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.MapPost("test", () =>
{
    return TypedResults.ServerSentEvents(Events());

    async IAsyncEnumerable<string> Events()
    {
        foreach (var number in Enumerable.Range(1, 200))
        {
            yield return $"{number}";
            await Task.Delay(1000);
        }
    }
});

app.Run();