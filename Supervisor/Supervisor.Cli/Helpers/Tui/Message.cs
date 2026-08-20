namespace Supervisor.Cli.Helpers.Tui;

public static class Message
{
    public static void Info(string message, bool goToNextLine = true)
    {
        WriteWithIcon("ⓘ", message, ConsoleColor.Cyan, goToNextLine: goToNextLine);
    }

    public static void Success(string message, bool goToNextLine = true)
    {
        WriteWithIcon("✓", message, ConsoleColor.Green, goToNextLine: goToNextLine);
    }

    public static void Warning(string message, bool goToNextLine = true)
    {
        WriteWithIcon("‼", message, ConsoleColor.Yellow, goToNextLine: goToNextLine);
    }

    public static void Error(string message, bool goToNextLine = true)
    {
        WriteWithIcon("✗", message, ConsoleColor.Red, goToNextLine: goToNextLine);
    }

    public static void Disable(string message, bool goToNextLine = true)
    {
        WriteWithIcon("", message, ConsoleColor.DarkGray, ConsoleColor.DarkGray, goToNextLine);
    }

    public static void WriteLine(string message, ConsoleColor? color = null)
    {
        var oldColor = Console.ForegroundColor;

        if (color is not null) Console.ForegroundColor = color.Value;

        Console.WriteLine(message);

        Console.ForegroundColor = oldColor;
    }

    public static void Write(string message, ConsoleColor? color = null)
    {
        var oldColor = Console.ForegroundColor;

        if (color is not null) Console.ForegroundColor = color.Value;

        Console.Write(message);

        Console.ForegroundColor = oldColor;
    }

    public static void WriteWithIcon(
        string icon,
        string message,
        ConsoleColor iconColor,
        ConsoleColor? messageColor = null, bool goToNextLine = true)
    {
        Write($"{icon} ", iconColor);

        if (goToNextLine) WriteLine(message, messageColor);
        else Write(message, messageColor);
    }
}