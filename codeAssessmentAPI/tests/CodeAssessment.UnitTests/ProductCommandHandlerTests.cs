using CodeAssessment.Application.Commands;
using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Validators;
using FluentValidation;
using Moq;

namespace CodeAssessment.UnitTests;

public sealed class ProductCommandHandlerTests
{
    [Fact]
    public async Task CreateProduct_WithValidCommand_AddsAndReturnsProduct()
    {
        // Arrange
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.AddAsync(It.IsAny<CodeAssessment.Domain.Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repository.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new CreateProductCommandHandler(repository.Object, new CreateProductRequestValidator());

        // Act
        var result = await handler.HandleAsync(new CreateProductCommand("Product 1", 10));

        // Assert
        Assert.Equal("Product 1", result.Name);
        Assert.Equal(10, result.Stock);
        Assert.True(result.Active);
        repository.Verify(item => item.AddAsync(It.Is<CodeAssessment.Domain.Product>(product =>
            product.Name == "Product 1" && product.Stock == 10), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidCommand_ThrowsValidationExceptionAndDoesNotPersist()
    {
        // Arrange
        var repository = TestData.RepositoryMock();
        var handler = new CreateProductCommandHandler(repository.Object, new CreateProductRequestValidator());

        // Act
        var action = () => handler.HandleAsync(new CreateProductCommand("", -1));

        // Assert
        await Assert.ThrowsAsync<ValidationException>(action);
        repository.Verify(item => item.AddAsync(It.IsAny<CodeAssessment.Domain.Product>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductExists_UpdatesAndReturnsProduct()
    {
        // Arrange
        var product = TestData.Product(stock: 5);
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        repository.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new UpdateProductCommandHandler(repository.Object, new UpdateProductRequestValidator());

        // Act
        var result = await handler.HandleAsync(new UpdateProductCommand(7, "Updated", 12));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
        Assert.Equal(12, result.Stock);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ReturnsNullWithoutSaving()
    {
        // Arrange
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CodeAssessment.Domain.Product?)null);
        var handler = new UpdateProductCommandHandler(repository.Object, new UpdateProductRequestValidator());

        // Act
        var result = await handler.HandleAsync(new UpdateProductCommand(7, "Updated", 12));

        // Assert
        Assert.Null(result);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductExists_DeactivatesAndSavesProduct()
    {
        // Arrange
        var product = TestData.Product();
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        repository.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new DeleteProductCommandHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new DeleteProductCommand(7));

        // Assert
        Assert.True(result);
        Assert.False(product.Active);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CodeAssessment.Domain.Product?)null);
        var handler = new DeleteProductCommandHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new DeleteProductCommand(7));

        // Assert
        Assert.False(result);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddStock_WhenProductExists_IncreasesStockAndSaves()
    {
        // Arrange
        var product = TestData.Product(stock: 10);
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        repository.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new AddStockCommandHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new AddStockCommand(7, 3));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(13, product.Stock);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DecrementStock_WhenQuantityExceedsStock_ThrowsDomainException()
    {
        // Arrange
        var product = TestData.Product(stock: 2);
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var handler = new DecrementStockCommandHandler(repository.Object);

        // Act
        var action = () => handler.HandleAsync(new DecrementStockCommand(7, 3));

        // Assert
        await Assert.ThrowsAsync<CodeAssessment.Domain.ProductDomainException>(action);
        Assert.Equal(2, product.Stock);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
