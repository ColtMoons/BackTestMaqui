using Domain.Abstractions;

namespace Application.Abstractions.Services;

public interface IDiscountService
{
    Task<Result<decimal>> GetDiscountAsync(Guid productId, CancellationToken cancellationToken = default);
}
