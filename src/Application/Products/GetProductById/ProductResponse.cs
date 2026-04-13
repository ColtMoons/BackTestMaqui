namespace Application.Products.GetProductById;

public sealed record ProductResponse(
    Guid ProductId,
    string Name,
    string StatusName,
    int Stock,
    string? Description,
    decimal Price,
    decimal Discount,
    decimal FinalPrice);
