using Application.Products.GetProductById;
using Domain.Abstractions;
using MediatR;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Products.GetById;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<ProductResponse> result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Products)
            .WithName("GetProductById");
    }
}
