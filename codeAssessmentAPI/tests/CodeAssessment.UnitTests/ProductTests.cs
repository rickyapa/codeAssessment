using CodeAssessment.Domain;

namespace CodeAssessment.UnitTests;

public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidValues_TrimsNameAndAppliesDefaults()
    {
        // Arrange
        var createdAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var product = Product.Create(" Product 1 ", 10, createdAt);

        // Assert
        Assert.Equal("Product 1", product.Name);
        Assert.Equal(10, product.Stock);
        Assert.Equal(createdAt, product.CreatedAt);
        Assert.Null(product.UpdatedAt);
        Assert.True(product.Active);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsDomainException()
    {
        // Arrange
        const string name = " ";

        // Act
        var action = () => Product.Create(name);

        // Assert
        var exception = Assert.Throws<ProductDomainException>(action);
        Assert.Contains("name is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithNegativeStock_ThrowsDomainException()
    {
        // Arrange
        const int stock = -1;

        // Act
        var action = () => Product.Create("Product 1", stock);

        // Assert
        Assert.Throws<ProductDomainException>(action);
    }

    [Fact]
    public void Update_WithValidValues_ChangesNameAndStock()
    {
        // Arrange
        var product = TestData.Product();

        // Act
        product.Update(" Updated product ", 25);

        // Assert
        Assert.Equal("Updated product", product.Name);
        Assert.Equal(25, product.Stock);
        Assert.NotNull(product.UpdatedAt);
        Assert.True(product.Active);
    }

    [Fact]
    public void AddStock_WithPositiveQuantity_IncreasesStock()
    {
        // Arrange
        var product = TestData.Product(stock: 10);

        // Act
        product.AddStock(5);

        // Assert
        Assert.Equal(15, product.Stock);
        Assert.NotNull(product.UpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddStock_WithNonPositiveQuantity_ThrowsDomainException(int quantity)
    {
        // Arrange
        var product = TestData.Product();

        // Act
        var action = () => product.AddStock(quantity);

        // Assert
        Assert.Throws<ProductDomainException>(action);
    }

    [Fact]
    public void DecrementStock_WithAvailableQuantity_DecreasesStock()
    {
        // Arrange
        var product = TestData.Product(stock: 10);

        // Act
        product.DecrementStock(4);

        // Assert
        Assert.Equal(6, product.Stock);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void DecrementStock_WhenQuantityExceedsStock_ThrowsDomainException()
    {
        // Arrange
        var product = TestData.Product(stock: 3);

        // Act
        var action = () => product.DecrementStock(4);

        // Assert
        Assert.Throws<ProductDomainException>(action);
        Assert.Equal(3, product.Stock);
    }

    [Fact]
    public void Deactivate_SetsProductInactive()
    {
        // Arrange
        var product = TestData.Product();

        // Act
        product.Deactivate();

        // Assert
        Assert.False(product.Active);
        Assert.NotNull(product.UpdatedAt);
    }
}
