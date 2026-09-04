using System.Diagnostics;
using Supervisor.Application.Services.ProcessProfile;

namespace Supervisor.Application.Services;

public class InterfaceProcessProfile : ProcessProfile.ProcessProfile;

public class InterfaceProcessManager(
    PipeClient pipeClient,
    ProfileService<ProcessProfile.ProcessProfile> profileService) : ProcessManager<ProcessProfile.ProcessProfile>(pipeClient, profileService)
{
    protected override string FilePath { get; set; }
    
    protected override void OnOutput(object? sender, DataReceivedEventArgs e)
    {
    }

    protected override void OnError(object? sender, DataReceivedEventArgs e)
    {
        
    }
}