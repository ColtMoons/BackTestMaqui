using Domain.Products;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class ProductRepository(ApplicationDbContext dbContext) 
    : Repository<Product>(dbContext), IProductRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Product>().AnyAsync(p => p.Name == name, cancellationToken);
    }
}
