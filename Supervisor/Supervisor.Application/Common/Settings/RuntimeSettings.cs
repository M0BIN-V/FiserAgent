namespace Supervisor.Application.Common.Settings;

public class RuntimeSettings
{
    public const string ProcessProfileFileName = "process.profile.json";
    public readonly string DirectoryName = "Runtime";
    public readonly string ManifestFileName = "manifest.json";

    public RuntimeSettings(SupervisorSettings supervisorSettings)
    {
        DirectoryPath = Path.Combine(supervisorSettings.InstallationDirectory, DirectoryName);

        ManifestPath = Path.Combine(DirectoryPath, ManifestFileName);

        BinaryPath = Path.Combine(DirectoryPath, OperatingSystem.IsWindows() ? "Fiser.Runtime.exe" : "Fiser.Runtime");

        ProcessProfile = Path.Combine(DirectoryPath, ProcessProfileFileName);
    }

    public string BinaryPath { get; private set; }

    public string ManifestPath { get; private set; }

    public string ProcessProfile { get; private set; }

    public string DirectoryPath { get; }
}