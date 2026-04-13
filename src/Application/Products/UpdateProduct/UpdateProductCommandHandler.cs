using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Products;

namespace Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.Id));
        }

        product.Update(request.Name, request.Status, request.Stock, request.Description, request.Price);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
