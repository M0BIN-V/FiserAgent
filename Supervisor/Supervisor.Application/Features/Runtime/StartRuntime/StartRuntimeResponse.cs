using OneOf;
using Supervisor.Application.Common.Errors;

namespace Supervisor.Application.Features.Runtime.StartRuntime;

[GenerateOneOf]
public partial class StartRuntimeResponse : OneOfBase<
    Uri,
    RuntimeIsNotInstalledError ,
    RuntimeIsAlreadyRunningError>;