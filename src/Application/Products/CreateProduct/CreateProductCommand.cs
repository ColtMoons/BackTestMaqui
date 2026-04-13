using Application.Abstractions.Messaging;

namespace Application.Products.CreateProduct;

public sealed record CreateProductCommand(string Name, int Stock, string? Description, decimal Price) : ICommand<Guid>;
