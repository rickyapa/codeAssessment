using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Interfaces;
using CodeAssessment.Application.Mappings;
using CodeAssessment.Domain;
using FluentValidation;

namespace CodeAssessment.Application.Commands;

public sealed record CreateProductCommand(string Name, int Stock = 0);

public sealed class CreateProductCommandHandler(
    IProductRepository repository,
    IValidator<CreateProductRequest> validator)
{
    public async Task<ProductDto> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var request = new CreateProductRequest(command.Name, command.Stock);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var product = Product.Create(command.Name, command.Stock);
        await repository.AddAsync(product, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ProductDtoMapper.Map(product);
    }
}
