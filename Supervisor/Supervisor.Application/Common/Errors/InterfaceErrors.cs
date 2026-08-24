namespace Supervisor.Application.Common.Errors;

public record InterfaceNotFoundError(string UniqueName) : Error($"interface '{UniqueName}' not found");