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

    private static void WriteColored(
        string icon,
        string message,
        ConsoleColor color)
    {
        var oldColor = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.Write($"{icon} ");

        Console.ForegroundColor = oldColor;
        Console.WriteLine(message);
    }

}