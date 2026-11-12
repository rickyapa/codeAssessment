using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Validators;

namespace CodeAssessment.UnitTests;

public sealed class ProductValidatorTests
{
    [Fact]
    public void CreateProductRequest_WithValidValues_PassesValidation()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = new CreateProductRequest("Product 1", 10);

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateProductRequest_WithMissingName_FailsValidation(string name)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = new CreateProductRequest(name, 0);

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Name));
    }

    [Fact]
    public void CreateProductRequest_WithNegativeStock_FailsValidation()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = new CreateProductRequest("Product 1", -1);

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Stock));
    }

    [Fact]
    public void UpdateProductRequest_WithValidValues_PassesValidation()
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();
        var request = new UpdateProductRequest("Product 1", 10);

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateProductRequest_WithNegativeStock_FailsValidation()
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();
        var request = new UpdateProductRequest("Product 1", -1);

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProductRequest.Stock));
    }
}
