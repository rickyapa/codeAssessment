namespace CodeAssessment.Domain;

public sealed class Product
{
    private Product()
    {
    }

    private Product(string name, int stock, DateTime createdAt, bool active)
    {
        Name = name;
        Stock = stock;
        CreatedAt = createdAt;
        Active = active;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool Active { get; private set; }

    public static Product Create(
        string name,
        int stock = 0,
        DateTime? createdAt = null)
    {
        ValidateName(name);
        ValidateStock(stock);

        return new Product(name.Trim(), stock, createdAt ?? DateTime.UtcNow, true);
    }

    public void Update(string name, int stock)
    {
        ValidateName(name);
        ValidateStock(stock);

        Name = name.Trim();
        Stock = stock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity)
    {
        ValidateQuantity(quantity);
        Stock = checked(Stock + quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecrementStock(int quantity)
    {
        ValidateQuantity(quantity);
        if (quantity > Stock)
        {
            throw new ProductDomainException("Product stock cannot become negative.");
        }

        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Active = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
        {
            throw new ProductDomainException("Product name is required and must be 200 characters or fewer.");
        }
    }

    private static void ValidateStock(int stock)
    {
        if (stock < 0)
        {
            throw new ProductDomainException("Product stock cannot be negative.");
        }
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ProductDomainException("Stock quantity must be greater than zero.");
        }
    }
}
