using Domain.Abstractions;

namespace Domain.Products;

public sealed class Product : Entity
{
    private Product(Guid id,
        string name,
        ProductStatus status,
        int stock,
        string? description,
        decimal price,
        DateTime createdAt) : base(id)
    {
        Name = name;
        Status = status;
        Stock = stock;
        Description = description;
        Price = price;
        CreatedAt = createdAt;
    }

    public string Name { get; private set; }
    public ProductStatus Status { get; private set; }
    public int Stock { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Product Create(string name,
        ProductStatus status,
        int stock,
        string? description,
        decimal price,
        DateTime createdAt)
    {
        return new Product(Guid.CreateVersion7(), name, status, stock, description, price, createdAt);
    }
}
