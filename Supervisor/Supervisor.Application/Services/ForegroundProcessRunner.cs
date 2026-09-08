using System.Diagnostics;

namespace Supervisor.Application.Services;

public sealed class ForegroundProcessRunner(ILogger<ForegroundProcessRunner> logger)
{
    public System.Diagnostics.Process Run(string filePath, params string[] arguments)
    {
        logger.LogDebug($"Starting process: {filePath}");
        if (!File.Exists(filePath))
            throw new FileNotFoundException("The executable file was not found.", filePath);

        var startInfo = new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = false,

            RedirectStandardInput = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,

            CreateNoWindow = false
        };

        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        var process = System.Diagnostics.Process.Start(startInfo)
                      ?? throw new InvalidOperationException($"Failed to start process: {filePath}");

        while (!process.HasExited)
        {
        }

        return process;
    }
}