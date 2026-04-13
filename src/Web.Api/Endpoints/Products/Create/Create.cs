using Application.Products.CreateProduct;
using Domain.Abstractions;
using MediatR;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Products.Create;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/products",
                async (CreateProductRequest request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var command = new CreateProductCommand(request.Name, request.Stock, request.Description,
                        request.Price);

                    Result<Guid> result = await sender.Send(command, cancellationToken);

                    return result.Match(
                        onSuccess: id => Results.CreatedAtRoute("GetProductById", new { id }, id),
                        onFailure: CustomResults.Problem);
                })
            .WithTags(Tags.Products)
            .Produces<Guid>(StatusCodes.Status201Created);
    }
}
