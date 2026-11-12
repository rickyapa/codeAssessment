using CodeAssessment.Application.Interfaces;
using CodeAssessment.Domain;
using CodeAssessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeAssessment.Infrastructure.Repositories;

public sealed class ProductRepository(ProductDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Products.SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Active)
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> SearchByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Active && product.Name.Contains(name))
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> GetByStockRangeAsync(int min, int max, CancellationToken cancellationToken = default) =>
        await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Active && product.Stock >= min && product.Stock <= max)
            .OrderBy(product => product.Stock)
            .ThenBy(product => product.Name)
            .ToListAsync(cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        dbContext.Products.AddAsync(product, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
