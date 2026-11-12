using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;

namespace CodeAssessment.Application.Queries;

public sealed record GetProductsQuery;

public sealed class GetProductsQueryHandler(IProductRepository repository)
{
    public async Task<IReadOnlyList<ProductDto>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.Select(ProductDtoMapper.Map).ToList();
    }
}
