using Microsoft.Extensions.AI;
using Runtime.Application.Common.Abstractions;

namespace Runtime.Application.Features.GetToolList;

public record GetToolListRequest;

public record GetToolListResponse(List<AIFunction> tools);

public class GetToolListHandler() : Handler<GetToolListRequest, GetToolListResponse>
{
    public override Task<GetToolListResponse> HandleAsync(GetToolListRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}