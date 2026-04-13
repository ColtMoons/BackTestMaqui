using Application.Abstractions.Services;
using Domain.Abstractions;
using Domain.Products;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discounts;

public class DiscountService(IDiscountApiClient client, ILogger<DiscountService> logger) : IDiscountService
{
    public async Task<Result<decimal>> GetDiscountAsync(
        Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            List<DiscountApiResponse> response = await client.GetDiscountAsync(productId, cancellationToken);

            if (response.Count == 0)
            {
                return Result.Success(0m);
            }
            
            DiscountApiResponse discount = response[0];
            
            if (discount.Discount is >= 0 and <= 100)
            {
                return Result.Success(discount.Discount);
            }

            logger.LogWarning(
                "Discount {Discount} out of range for product {ProductId}",
                discount.Discount, productId);

            return Result.Failure<decimal>(ProductErrors.DiscountOutOfRange(discount.Discount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to get discount for product {ProductId}", productId);
            return Result.Failure<decimal>(ProductErrors.ServiceUnavailable);
        }
    }
}
