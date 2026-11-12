using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;

namespace CodeAssessment.Application.Commands;

public sealed record DecrementStockCommand(int Id, int Quantity);

public sealed class DecrementStockCommandHandler(IProductRepository repository)
{
    public async Task<ProductDto?> HandleAsync(DecrementStockCommand command, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        product.DecrementStock(command.Quantity);
        await repository.SaveChangesAsync(cancellationToken);
        return ProductDtoMapper.Map(product);
    }
}
