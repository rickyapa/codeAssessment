using CodeAssessment.Domain;

namespace CodeAssessment.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByStockRangeAsync(int min, int max, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
