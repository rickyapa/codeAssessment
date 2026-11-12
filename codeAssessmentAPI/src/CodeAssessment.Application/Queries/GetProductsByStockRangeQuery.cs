using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;
using CodeAssessment.Domain;

namespace CodeAssessment.Application.Queries;

public sealed record GetProductsByStockRangeQuery(int Min, int Max);

public sealed class GetProductsByStockRangeQueryHandler(IProductRepository repository)
{
    public async Task<IReadOnlyList<ProductDto>> HandleAsync(GetProductsByStockRangeQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Min < 0)
        {
            throw new ProductDomainException("Minimum stock cannot be negative.");
        }

        if (query.Max < query.Min)
        {
            throw new ProductDomainException("Maximum stock must be greater than or equal to minimum stock.");
        }

        var products = await repository.GetByStockRangeAsync(query.Min, query.Max, cancellationToken);
        return products.Select(ProductDtoMapper.Map).ToList();
    }
}
