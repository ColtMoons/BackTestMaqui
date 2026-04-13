using Domain.Abstractions;

namespace Domain.Products;

public static class ProductErrors
{
    public static Error AlreadyExistsWithName(string name) => Error.Conflict("Product.AlreadyExists", $"Product already exists with name '{name}'");
    public static Error NotFound(Guid id) => Error.NotFound("Product.NotFound", $"Product with id '{id}' not found");
    public static Error DiscountOutOfRange(decimal discount) => Error.Failure("Product.DiscountOutOfRange", $"Discount must be between 0 and 100, but was {discount}");
    public static Error ServiceUnavailable => Error.Failure("Product.ServiceUnavailable", "Discount service is currently unavailable");
}
