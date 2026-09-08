namespace Supervisor.Application.Features.Runtime.StartRuntime;

public record StartRuntimeRequest(string ApiKey, string ModelName, string Endpoint);