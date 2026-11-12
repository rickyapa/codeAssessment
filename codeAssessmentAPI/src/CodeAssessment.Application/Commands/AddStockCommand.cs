using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;

namespace CodeAssessment.Application.Commands;

public sealed record AddStockCommand(int Id, int Quantity);

public sealed class AddStockCommandHandler(IProductRepository repository)
{
    public async Task<ProductDto?> HandleAsync(AddStockCommand command, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        product.AddStock(command.Quantity);
        await repository.SaveChangesAsync(cancellationToken);
        return ProductDtoMapper.Map(product);
    }
}
