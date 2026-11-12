namespace CodeAssessment.Domain;

public sealed class ProductDomainException : Exception
{
    public ProductDomainException(string message)
        : base(message)
    {
    }
}
