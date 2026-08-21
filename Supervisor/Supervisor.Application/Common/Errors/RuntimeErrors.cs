namespace Supervisor.Application.Common.Errors;

public record RuntimeAlreadyInstalledError() : Error("Runtime is already installed.");