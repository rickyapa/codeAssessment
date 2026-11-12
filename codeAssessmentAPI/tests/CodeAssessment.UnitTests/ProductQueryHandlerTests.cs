using CodeAssessment.Application.Queries;
using FluentValidation;
using Moq;

namespace CodeAssessment.UnitTests;

public sealed class ProductQueryHandlerTests
{
    [Fact]
    public async Task GetProducts_ReturnsProductsFromRepository()
    {
        // Arrange
        var products = new[] { TestData.Product("Product 1", 10), TestData.Product("Product 2", 1) };
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        var handler = new GetProductsQueryHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new GetProductsQuery());

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { "Product 1", "Product 2" }, result.Select(item => item.Name));
    }

    [Fact]
    public async Task GetProductById_WhenProductExists_ReturnsProductDto()
    {
        // Arrange
        var product = TestData.Product("Product 1", 10);
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var handler = new GetProductByIdQueryHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new GetProductByIdQuery(7));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Product 1", result.Name);
        Assert.Equal(10, result.Stock);
    }

    [Fact]
    public async Task GetProductById_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CodeAssessment.Domain.Product?)null);
        var handler = new GetProductByIdQueryHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new GetProductByIdQuery(7));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchProducts_WithPartialName_UsesTrimmedSearchAndReturnsMatches()
    {
        // Arrange
        var products = new[] { TestData.Product("Product 1", 10) };
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.SearchByNameAsync("duct", It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        var handler = new SearchProductsQueryHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new SearchProductsQuery(" duct "));

        // Assert
        Assert.Single(result);
        Assert.Equal("Product 1", result[0].Name);
        repository.Verify(item => item.SearchByNameAsync("duct", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchProducts_WithBlankName_ThrowsValidationException()
    {
        // Arrange
        var repository = TestData.RepositoryMock();
        var handler = new SearchProductsQueryHandler(repository.Object);

        // Act
        var action = () => handler.HandleAsync(new SearchProductsQuery(" "));

        // Assert
        await Assert.ThrowsAsync<ValidationException>(action);
    }

    [Fact]
    public async Task GetProductsByStockRange_WithValidRangeReturnsMatchingProducts()
    {
        // Arrange
        var products = new[] { TestData.Product("Product 1", 10) };
        var repository = TestData.RepositoryMock();
        repository.Setup(item => item.GetByStockRangeAsync(5, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        var handler = new GetProductsByStockRangeQueryHandler(repository.Object);

        // Act
        var result = await handler.HandleAsync(new GetProductsByStockRangeQuery(5, 10));

        // Assert
        Assert.Single(result);
        Assert.Equal(10, result[0].Stock);
        repository.Verify(item => item.GetByStockRangeAsync(5, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductsByStockRange_WhenMinimumExceedsMaximum_ThrowsDomainException()
    {
        // Arrange
        var repository = TestData.RepositoryMock();
        var handler = new GetProductsByStockRangeQueryHandler(repository.Object);

        // Act
        var action = () => handler.HandleAsync(new GetProductsByStockRangeQuery(11, 10));

        // Assert
        await Assert.ThrowsAsync<CodeAssessment.Domain.ProductDomainException>(action);
    }
}
