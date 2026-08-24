namespace Supervisor.Application.Common.Options;

public class SupervisorOptions
{
    public string Directory = AppContext.BaseDirectory;
    public string InterfaceInstallationPath = Path.Combine(AppContext.BaseDirectory, "Interfaces");
}