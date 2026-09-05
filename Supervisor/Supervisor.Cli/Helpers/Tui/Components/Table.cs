using Spectre.Console;

namespace Supervisor.Cli.Helpers.Tui.Components;

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