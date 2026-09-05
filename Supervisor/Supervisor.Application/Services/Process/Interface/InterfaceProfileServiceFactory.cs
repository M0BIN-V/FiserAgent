namespace Supervisor.Application.Services.Process.Interface;

public class InterfaceProfileServiceFactory
{
    public InterfaceProfileService Create(string profileFilePath)
    {
        return new InterfaceProfileService(profileFilePath);
    }
}