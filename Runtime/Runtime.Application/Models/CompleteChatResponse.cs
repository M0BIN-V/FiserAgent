namespace Runtime.Application.Models;

public class CompleteChatResponse
{
    private CompleteChatResponse()
    {
    }

    public string? Text { get; private set; }
    public string? ToolName { get; private set; }
    public string? ToolViewName { get; private set; }
    public object? ToolResult { get; private set; }
    public ChatEventType Type { get; private set; }


    public static CompleteChatResponse CreateText(string text)
    {
        return new CompleteChatResponse
        {
            Text = text,
            Type = ChatEventType.Text
        };
    }

    public static CompleteChatResponse CreateToolCall(string name, string viewName)
    {
        return new CompleteChatResponse
        {
            ToolName = name,
            ToolViewName = viewName,
            Type = ChatEventType.ToolCall
        };
    }

    public static CompleteChatResponse CreateCompleted()
    {
        return new CompleteChatResponse
        {
            Type = ChatEventType.Completed
        };
    }

    public static CompleteChatResponse CreateToolResult(object result)
    {
        return new CompleteChatResponse
        {
            ToolResult = result,
            Type = ChatEventType.ToolResult
        };
    }
}