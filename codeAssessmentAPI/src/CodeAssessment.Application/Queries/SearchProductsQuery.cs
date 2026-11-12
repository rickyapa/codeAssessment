using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;
using FluentValidation;

namespace CodeAssessment.Application.Queries;

public sealed record SearchProductsQuery(string Name);

public sealed class SearchProductsQueryHandler(IProductRepository repository)
{
    public async Task<IReadOnlyList<ProductDto>> HandleAsync(SearchProductsQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Name))
        {
            throw new ValidationException("The name search parameter is required.");
        }

        var products = await repository.SearchByNameAsync(query.Name.Trim(), cancellationToken);
        return products.Select(ProductDtoMapper.Map).ToList();
    }
}
