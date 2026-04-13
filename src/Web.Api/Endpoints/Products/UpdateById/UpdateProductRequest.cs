using Domain.Products;

namespace Web.Api.Endpoints.Products.UpdateById;

public sealed record UpdateProductRequest(string Name, int Stock, string? Description, decimal Price, ProductStatus Status);
