using Domain.Products;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

internal sealed class ProductRepository(ApplicationDbContext dbContext) 
    : Repository<Product>(dbContext), IProductRepository
{
    
}
