using CodeAssessment.Application.DTOs;
using CodeAssessment.Domain;

namespace CodeAssessment.Application.Mappings;

public static class ProductDtoMapper
{
    public static ProductDto Map(Product product) => new(
        product.Id,
        product.Name,
        product.Stock,
        product.CreatedAt,
        product.UpdatedAt,
        product.Active);
}
