namespace Fiser.Supervisor.Helpers.Tui;

public sealed class ProgressBar(
    string title,
    int width = 40) : IProgress<double>
{
    public void Report(double value)
    {
        value = Math.Clamp(value, 0, 1);

        var percentage = value * 100;
        var filled = (int)(width * value);

        var bar =
            new string('█', filled) +
            new string('░', width - filled);

        Console.Write(
            $"\r{title} [{bar}] {percentage,6:0.0}%");

        if (value >= 1)
            Console.WriteLine();
    }
}