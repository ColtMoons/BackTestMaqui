using Domain.Abstractions;

namespace Domain.Products;

public static class ProductErrors
{
    public static Error AlreadyExistsWithName(string name) => Error.Conflict("Product.AlreadyExists", $"Product already exists with name '{name}'");
}
