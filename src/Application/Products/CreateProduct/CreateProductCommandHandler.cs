using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Products;

namespace Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await productRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            return Result.Failure<Guid>(ProductErrors.AlreadyExistsWithName(request.Name));
        }
        
        var product = Product.Create(
            request.Name,
            request.Stock,
            request.Description, 
            request.Price,
            DateTime.UtcNow);
        
        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
