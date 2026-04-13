using MediatR;

namespace Web.Api.Endpoints.Products.GetById;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                throw new NotImplementedException();
            })
            .WithTags(Tags.Products)
            .WithName("GetProductById");
    }
}
