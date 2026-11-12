using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;
using FluentValidation;

namespace CodeAssessment.Application.Commands;

public sealed record UpdateProductCommand(int Id, string Name, int Stock);

public sealed class UpdateProductCommandHandler(
    IProductRepository repository,
    IValidator<UpdateProductRequest> validator)
{
    public async Task<ProductDto?> HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        var request = new UpdateProductRequest(command.Name, command.Stock);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        product.Update(command.Name, command.Stock);
        await repository.SaveChangesAsync(cancellationToken);
        return ProductDtoMapper.Map(product);
    }
}
