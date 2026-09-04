using Spectre.Console;

namespace Supervisor.Cli.Helpers.Tui;

public static class Message
{
    public static void SuggestCommand(string command, string reason)
    {
        Info("run ", false);

        var style = new Style(
            Color.Cyan,
            decoration: Decoration.Bold);

        var text = new Text($"'fiser {command}' ", style);

        AnsiConsole.Write(text);

        Write($"to {reason}");
    }

    public static void Info(string message, bool goToNextLine = true)
    {
        WriteWithIcon("ⓘ", message, Color.Cyan, goToNextLine: goToNextLine);
    }

    public static void Success(string message, bool goToNextLine = true)
    {
        WriteWithIcon("✓", message, Color.Green, goToNextLine: goToNextLine);
    }

    public static void Warning(string message, bool goToNextLine = true)
    {
        WriteWithIcon("‼", message, Color.Yellow, goToNextLine: goToNextLine);
    }

    public static void Error(string message, bool goToNextLine = true)
    {
        WriteWithIcon("✗", message, Color.Red, goToNextLine: goToNextLine);
    }

    public static void Disable(string message, bool goToNextLine = true)
    {
        WriteWithIcon("", message, Color.Default, Color.Default, goToNextLine);
    }

    public static void Write(string message, Color? color = null)
    {
        var style = new Style(color ?? Color.Default);
        var text = new Text(message, style);

        AnsiConsole.Write(text);
    }

    public static void WriteWithIcon(
        string icon,
        string message,
        Color iconColor,
        Color? messageColor = null, bool goToNextLine = true)
    {
        var iconStyle = new Style(iconColor, decoration: Decoration.Bold);
        var iconText = new Text($"{icon} ", iconStyle);

        var messageStyle = new Style(messageColor ?? Color.Default);
        var messageText = new Text(message, messageStyle);

        AnsiConsole.Write(iconText);
        AnsiConsole.Write(messageText);

        if (goToNextLine) Console.WriteLine();
    }
}