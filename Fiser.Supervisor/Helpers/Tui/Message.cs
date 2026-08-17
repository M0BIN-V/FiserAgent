namespace Fiser.Supervisor.Helpers.Tui;

public static class Message
{
    // ─────────────────────────────────────────────
    // Messages
    // ─────────────────────────────────────────────

    public static void Info(string message)
    {
        WriteColored("ⓘ", message, ConsoleColor.Cyan);
    }

    public static void Success(string message)
    {
        WriteColored("✓", message, ConsoleColor.Green);
    }

    public static void Warning(string message)
    {
        WriteColored("‼", message, ConsoleColor.Yellow);
    }

    public static void Error(string message)
    {
        WriteColored("✗", message, ConsoleColor.Red);
    }

    public static void Disable(string message)
    {
        WriteColored("", message, ConsoleColor.DarkGray, ConsoleColor.DarkGray);
    }


    public static void WriteColored(
        string icon,
        string message,
        ConsoleColor iconColor,
        ConsoleColor? color = null)
    {
        var oldColor = Console.ForegroundColor;

        Console.ForegroundColor = iconColor;
        Console.Write($"{icon} ");

        Console.ForegroundColor = color ?? oldColor;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}