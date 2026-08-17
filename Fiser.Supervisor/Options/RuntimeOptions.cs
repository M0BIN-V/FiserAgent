namespace Fiser.Supervisor.Options;

public class RuntimeOptions
{
    public RuntimeOptions()
    {
        FolderPath = Path.Combine(AppContext.BaseDirectory, FolderName);

        ManifestPath = Path.Combine(FolderPath, ManifestFileName);

        FilePath = Path.Combine(FolderPath, OperatingSystem.IsWindows() ? "Fiser.Runtime.exe" : "Fiser.Runtime");

        ProcessProfile = Path.Combine(FolderPath, "process.profile.json");
    }


    public string FolderName { get; init; } = "Runtime";

    public string ManifestFileName { get; init; } = "manifest.json";

    public string FilePath { get; init; }

    public string ProcessProfile { get; init; }

    public string FolderPath { get; init; }

    public string ManifestPath { get; init; }
}