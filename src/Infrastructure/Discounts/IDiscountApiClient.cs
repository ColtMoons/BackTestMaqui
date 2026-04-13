using Refit;

namespace Infrastructure.Discounts;

public interface IDiscountApiClient
{
    [Get("/discounts")]
    Task<List<DiscountApiResponse>> GetDiscountAsync(Guid productId, CancellationToken cancellationToken = default);
}
