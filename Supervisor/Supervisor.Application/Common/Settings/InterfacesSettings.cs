namespace Supervisor.Application.Common.Settings;

public class InterfacesSettings(SupervisorSettings supervisorSettings)
{
    public readonly string InstallationDirectory = Path.Combine(supervisorSettings.InstallationDirectory, "Interfaces");
    public readonly string ManifestFileName = "manifest.json";
    public readonly string ProcessProfileFileName = "process.profile.json";

    public string GenerateInstallationPath(string uniqueName)
    {
        return Path.Combine(InstallationDirectory, uniqueName);
    }
}