using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Supervisor.Application.Services.ProcessProfile;

namespace Supervisor.Application.Services;

public class InterfaceProcessProfile : ProcessProfile.ProcessProfile;

public class InterfaceProcessManager(
    PipeClient pipeClient,
    ILogger<ProcessManager<ProcessProfile.ProcessProfile>> baseLogger,
    ProfileService<ProcessProfile.ProcessProfile> profileService) 
    : ProcessManager<ProcessProfile.ProcessProfile>(baseLogger, pipeClient, profileService)
{
    protected override string FilePath { get; set; }
    
    protected override void OnOutput(object? sender, DataReceivedEventArgs e)
    {
    }

    protected override void OnError(object? sender, DataReceivedEventArgs e)
    {
        
    }
}