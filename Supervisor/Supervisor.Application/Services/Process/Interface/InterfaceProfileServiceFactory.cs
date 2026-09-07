using Supervisor.Application.Common.Settings;

namespace Supervisor.Application.Services.Process.Interface;

public class InterfaceProfileServiceFactory(InterfacesSettings settings)
{
    public InterfaceProfileService Create(string uniqueName)
    {
        var profileFilePath = Path.Combine(
            settings.InstallationDirectory,
            uniqueName,
            settings.ProcessProfileFileName);

        return new InterfaceProfileService(profileFilePath);
    }
}