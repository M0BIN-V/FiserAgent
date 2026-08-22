using System.ComponentModel.Design;

namespace Supervisor.Application.Common.Errors;

public record RuntimeAlreadyInstalledError() : Error("Runtime is already installed.");

public record RuntimeIsNotInstalledError():Error("Runtime is not installed.");

public record RuntimeIsAlreadyRunningError(): Error("Runtime is already running");