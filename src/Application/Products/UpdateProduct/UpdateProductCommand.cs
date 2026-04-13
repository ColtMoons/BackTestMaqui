using Application.Abstractions.Messaging;
using Domain.Products;

namespace Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(Guid Id, string Name, ProductStatus Status, int Stock, string? Description, decimal Price) : ICommand;
