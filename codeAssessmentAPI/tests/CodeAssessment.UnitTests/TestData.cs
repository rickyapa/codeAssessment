using CodeAssessment.Application.Interfaces;
using Moq;

namespace CodeAssessment.UnitTests;

internal static class TestData
{
    public static CodeAssessment.Domain.Product Product(string name = "Product 1", int stock = 10)
    {
        return CodeAssessment.Domain.Product.Create(name, stock);
    }

    public static Mock<IProductRepository> RepositoryMock()
    {
        return new Mock<IProductRepository>(MockBehavior.Strict);
    }
}
