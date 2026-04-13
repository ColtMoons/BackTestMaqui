using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Domain.Abstractions;
using Domain.Products;

namespace Application.Products.GetProductById;

internal sealed class GetProductByIdQueryHandler(IProductRepository repository, IDiscountService discountService, IStatusCacheService statusCacheService) : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Product? product = await repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product is null)
        {
            return Result.Failure<ProductResponse>(ProductErrors.NotFound(request.Id));
        }
        
        Result<decimal> discountResult = await discountService.GetDiscountAsync(product.Id, cancellationToken);
        
        decimal discount = discountResult.IsSuccess ? discountResult.Value : 0m;
        
        string statusName = await statusCacheService.GetStatusNameAsync((int)product.Status, cancellationToken);
        
        decimal finalPrice = product.Price * (100 - discount) / 100;
        
        return new ProductResponse(
            product.Id,
            product.Name,
            statusName,
            product.Stock, 
            product.Description,
            product.Price,
            discount,
            finalPrice);
    }
}
