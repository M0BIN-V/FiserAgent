namespace Runtime.WebApi;

public class CompletionResponse
{
    private CompletionResponse()
    {
    }

    public string? Text { get; private set; }
    public string? ToolName { get; private set; }
    public string? ToolViewName { get; private set; }
    public object? ToolResult { get; private set; }
    public ChatEventType Type { get; private set; }


    public static CompletionResponse CreateText(string text)
    {
        return new CompletionResponse
        {
            Text = text,
            Type = ChatEventType.Text
        };
    }

    public static CompletionResponse CreateToolCall(string name, string viewName)
    {
        return new CompletionResponse
        {
            ToolName = name,
            ToolViewName = viewName,
            Type = ChatEventType.ToolCall
        };
    }

    public static CompletionResponse CreateCompleted()
    {
        return new CompletionResponse
        {
            Type = ChatEventType.Completed
        };
    }

    public static CompletionResponse CreateToolResult(object result)
    {
        return new CompletionResponse
        {
            ToolResult = result,
            Type = ChatEventType.ToolResult
        };
    }
}