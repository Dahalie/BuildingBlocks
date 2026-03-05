using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Api.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
