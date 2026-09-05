namespace Supervisor.Application.Services.Process;

public abstract class ProcessProfile
{
    public string PipeName { get; set; } = null!;
    public int? ProcessId { get; set; }
    public string? ProcessName { get; set; }
}