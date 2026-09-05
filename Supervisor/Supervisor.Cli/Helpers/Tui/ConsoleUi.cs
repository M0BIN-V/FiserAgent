using Spectre.Console;

namespace Supervisor.Cli.Helpers.Tui;

public static class ConsoleUi
{
    public static string Select(string title, IReadOnlyList<string> items)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(items));
    }

    public static Task<T> StartProgress<T>(string title, Func<IProgress<ProgressUpdate>, Task<T>> action)
    {
        return AnsiConsole.Progress().StartAsync(ctx =>
        {
            var task = ctx.AddTask(title);

            var progress = new Progress<ProgressUpdate>(update =>
            {
                task.MaxValue = update.Total;
                task.Value = update.Current;

                if (!string.IsNullOrWhiteSpace(update.Message)) task.Description = update.Message;
            });

            return action(progress);
        });
    }


    public static Task StartSpinnerAsync(string message, Func<Task> func)
    {
        return AnsiConsole.Status()
            .Spinner(Spinner.Known.DotsCircle)
            .StartAsync("Checking runtime status...", ctx => func());
    }

    public static Task<T> StartSpinnerAsync<T>(string message, Func<Task<T>> func)
    {
        return AnsiConsole.Status()
            .Spinner(Spinner.Known.DotsCircle)
            .StartAsync("Checking runtime status...", ctx => func());
    }
}

public class Table
{
    public List<string> Columns { get; } = [];
    public List<List<string>> Rows { get; } = [];

    public Table AddColumns(params string[] columns)
    {
        Columns.AddRange(columns);
        return this;
    }

    public void AddRow(params string[] values)
    {
        Rows.Add(values.ToList());
    }

    public void Print()
    {
        var table = new Spectre.Console.Table();

        table.SimpleBorder();

        table.BorderColor(Color.Cyan);


        foreach (var column in Columns)
            table.AddColumn(column, col => col.Centered());

        foreach (var row in Rows)
            table.AddRow(row.ToArray());

        AnsiConsole.Write(table);
    }
}