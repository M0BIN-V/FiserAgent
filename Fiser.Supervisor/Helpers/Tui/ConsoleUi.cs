namespace Fiser.Supervisor.Helpers.Tui;

public static class ConsoleUi
{
    // ─────────────────────────────────────────────
    // Basic
    // ─────────────────────────────────────────────

    public static void Separator(int length = 50)
    {
        Console.WriteLine(new string('─', length));
    }

    public static void Clear()
    {
        Console.Clear();
    }

    public static void Pause(
        string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.Write(message);
        Console.ReadKey(true);
        Console.WriteLine();
    }

    public static void Banner(string title)
    {
        var width = Math.Max(title.Length + 6, 30);

        Console.WriteLine();
        Console.WriteLine($"╭{new string('─', width)}╮");
        Console.WriteLine($"│  {title.PadRight(width - 2)}│");
        Console.WriteLine($"╰{new string('─', width)}╯");
        Console.WriteLine();
    }


    // ─────────────────────────────────────────────
    // Input
    // ─────────────────────────────────────────────

    public static string Input(
        string question,
        string? defaultValue = null)
    {
        Console.Write(question);

        if (defaultValue is not null)
            Console.Write($" [{defaultValue}]");

        Console.Write(": ");

        var value = Console.ReadLine();

        if (string.IsNullOrEmpty(value) &&
            defaultValue is not null)
            return defaultValue;

        return value ?? string.Empty;
    }


    public static int InputInt(
        string question,
        int? defaultValue = null)
    {
        while (true)
        {
            var value = Input(
                question,
                defaultValue?.ToString());

            if (int.TryParse(value, out var result))
                return result;

            Message.Warning("Please enter a valid number.");
        }
    }


    public static string Password(string question)
    {
        Console.Write($"{question}: ");

        var password = new StringBuilder();

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
                break;

            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Remove(
                        password.Length - 1,
                        1);

                    Console.Write("\b \b");
                }

                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write('*');
            }
        }

        Console.WriteLine();

        return password.ToString();
    }


    // ─────────────────────────────────────────────
    // Confirm
    // ─────────────────────────────────────────────

    public static bool Confirm(
        string question,
        bool defaultValue = true)
    {
        var options = defaultValue
            ? "[Y/n]"
            : "[y/N]";

        while (true)
        {
            Console.Write($"{question} {options}: ");

            var input = Console.ReadLine()
                ?.Trim()
                .ToLowerInvariant();

            if (string.IsNullOrEmpty(input))
                return defaultValue;

            if (input is "y" or "yes")
                return true;

            if (input is "n" or "no")
                return false;

            Message.Warning("Please enter Y or N.");
        }
    }


    // ─────────────────────────────────────────────
    // Select
    // ─────────────────────────────────────────────

    public static T Select<T>(
        string title,
        IReadOnlyList<T> items)
    {
        if (items.Count == 0)
            throw new ArgumentException(
                "Items cannot be empty.",
                nameof(items));

        var selected = 0;

        Console.CursorVisible = false;

        var cursorLeft = Console.CursorLeft;
        var cursorTop = Console.CursorTop;

        try
        {
            while (true)
            {
                Console.SetCursorPosition(cursorLeft,cursorTop);

                Console.WriteLine(title);
                Console.WriteLine();

                for (var i = 0; i < items.Count; i++)
                {
                    if (i == selected)
                    {
                        Console.ForegroundColor =
                            ConsoleColor.Cyan;

                        Console.Write("❯ ");

                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("  ");
                    }

                    Console.WriteLine(items[i]);
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selected =
                            selected == 0
                                ? items.Count - 1
                                : selected - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selected =
                            selected == items.Count - 1
                                ? 0
                                : selected + 1;
                        break;

                    case ConsoleKey.Enter:
                        return items[selected];

                    case ConsoleKey.Escape:
                        throw new OperationCanceledException();
                }
            }
        }
        finally
        {
            Console.CursorVisible = true;
            Console.ResetColor();
        }
    }


    // ─────────────────────────────────────────────
    // Menu
    // ─────────────────────────────────────────────

    public static int Menu(
        string title,
        params string[] items)
    {
        return Select(
                title,
                items.Select((x, i) => $"{i + 1}. {x}")
                    .ToArray()) switch
            {
                var selected =>
                    Array.IndexOf(
                        items.Select((x, i) => $"{i + 1}. {x}")
                            .ToArray(),
                        selected)
            };
    }


    // ─────────────────────────────────────────────
    // List
    // ─────────────────────────────────────────────

    public static void List<T>(
        IEnumerable<T> items,
        string? title = null)
    {
        if (title is not null)
        {
            Console.WriteLine(title);
            Console.WriteLine();
        }

        foreach (var item in items) Console.WriteLine($"  • {item}");

        Console.WriteLine();
    }


    // ─────────────────────────────────────────────
    // Steps
    // ─────────────────────────────────────────────

    public static void Step(
        int current,
        int total,
        string message)
    {
        Console.WriteLine(
            $"[{current}/{total}] {message}");
    }


    // ─────────────────────────────────────────────
    // Progress
    // ─────────────────────────────────────────────

    public static ProgressBar Progress(
        string title,
        int width = 40)
    {
        return new ProgressBar(title, width);
    }


    // ─────────────────────────────────────────────
    // Spinner
    // ─────────────────────────────────────────────

    public static Spinner StartSpinner(
        string message)
    {
        return new Spinner(message);
    }


    // ─────────────────────────────────────────────
    // Table
    // ─────────────────────────────────────────────

    public static void Table(
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (headers.Count == 0)
            return;

        var widths = new int[headers.Count];

        for (var i = 0; i < headers.Count; i++)
        {
            widths[i] = headers[i].Length;

            foreach (var row in rows)
                if (i < row.Count)
                    widths[i] = Math.Max(
                        widths[i],
                        row[i].Length);
        }

        PrintTableSeparator(widths);

        PrintTableRow(headers, widths);

        PrintTableSeparator(widths);

        foreach (var row in rows)
            PrintTableRow(row, widths);

        PrintTableSeparator(widths);
    }


    private static void PrintTableSeparator(
        int[] widths)
    {
        Console.Write("+");

        foreach (var width in widths)
        {
            Console.Write(
                new string('-', width + 2));

            Console.Write("+");
        }

        Console.WriteLine();
    }


    private static void PrintTableRow(
        IReadOnlyList<string> values,
        int[] widths)
    {
        Console.Write("|");

        for (var i = 0; i < widths.Length; i++)
        {
            var value =
                i < values.Count
                    ? values[i]
                    : string.Empty;

            Console.Write(
                $" {value.PadRight(widths[i])} |");
        }

        Console.WriteLine();
    }
}