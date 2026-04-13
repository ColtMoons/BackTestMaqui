namespace Infrastructure.Discounts;

public sealed record DiscountApiResponse(Guid ProductId, decimal Discount);
