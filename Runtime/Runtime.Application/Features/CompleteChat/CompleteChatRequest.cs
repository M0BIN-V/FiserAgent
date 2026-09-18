using FluentValidation;
using Microsoft.Extensions.AI;
using Runtime.Application.Common.Abstractions;
using Runtime.Application.Installers;
using Runtime.Application.Models;

namespace Runtime.Application.Features.CompleteChat;

public class CompleteChatHandler(
    AgentFactory factory,
    IValidator<CompleteChatRequest> validator)
    : Handler<CompleteChatRequest, IAsyncEnumerable<CompleteChatResponse>>
{
    public override async Task<IAsyncEnumerable<CompleteChatResponse>> HandleAsync(CompleteChatRequest request,
        CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var agent = await factory.CreateAsync(ct);

        Store.Session ??= await agent.CreateSessionAsync(ct);

        return GetEvents();

        async IAsyncEnumerable<CompleteChatResponse> GetEvents()
        {
            await foreach (var update in agent.RunStreamingAsync(request.Message, Store.Session, cancellationToken: ct))
            foreach (var content in update.Contents)
            {
                var result = content switch
                {
                    TextContent t => string.IsNullOrEmpty(t.Text) ? null : CompleteChatResponse.CreateText(t.Text),
                    FunctionCallContent f => CompleteChatResponse.CreateToolCall(f.Name, f.Name),
                    FunctionResultContent fr => CompleteChatResponse.CreateToolResult(fr.Result),
                    _ => null
                };

                if (result is not null) yield return result;
            }
        }
    }
}

public record CompleteChatRequest(string Message, Guid SessionId);

public class CompleteChatRequestValidator : AbstractValidator<CompleteChatRequest>
{
    public CompleteChatRequestValidator()
    {
        RuleFor(r => r.Message)
            .NotEmpty()
            .MinimumLength(1);

        RuleFor(r => r.SessionId)
            .NotEmpty();
    }
}