namespace Supervisor.Application.Common.Errors;

public record InterfaceNotFoundError(string UniqueName) : Error($"interface '{UniqueName}' not found");

public record InterfaceIsAlreadyRunningError(string interfaceUniqueName)
    : Error($"interface '{interfaceUniqueName}' is already running");