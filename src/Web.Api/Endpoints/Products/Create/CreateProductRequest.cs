namespace Web.Api.Endpoints.Products.Create;

public sealed record CreateProductRequest(string Name, int Stock, string? Description, decimal Price);
