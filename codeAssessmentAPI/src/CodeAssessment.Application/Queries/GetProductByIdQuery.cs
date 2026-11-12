using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;

namespace CodeAssessment.Application.Queries;

public sealed record GetProductByIdQuery(int Id);

public sealed class GetProductByIdQueryHandler(IProductRepository repository)
{
    public async Task<ProductDto?> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(query.Id, cancellationToken);
        return product is null ? null : ProductDtoMapper.Map(product);
    }
}
