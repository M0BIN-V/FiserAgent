namespace Supervisor.Application.Services.Process.Interface;

public class InterfaceProfileService(string profileFilePath) : ProfileService<InterfaceProcessProfile>
{
    protected override string ProfileFilePath { get; } = profileFilePath;
}