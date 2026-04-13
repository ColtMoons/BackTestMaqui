using Application.Products.UpdateProduct;
using Domain.Abstractions;
using MediatR;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Products.UpdateById;

public sealed class UpdateById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/products/{id:guid}", async (Guid id, UpdateProductRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new UpdateProductCommand(id, request.Name, request.Status, request.Stock, request.Description, request.Price);
            Result result = await sender.Send(command, cancellationToken);
            
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Products);
    }
}
