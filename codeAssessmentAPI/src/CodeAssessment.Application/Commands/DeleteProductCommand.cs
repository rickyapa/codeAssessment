using CodeAssessment.Application.Interfaces;

namespace CodeAssessment.Application.Commands;

public sealed record DeleteProductCommand(int Id);

public sealed class DeleteProductCommandHandler(IProductRepository repository)
{
    public async Task<bool> HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        product.Deactivate();
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
