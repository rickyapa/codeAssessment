using CodeAssessment.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodeAssessment.Infrastructure.Persistence;

public sealed class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<int>("ProductIds")
            .StartsAt(100004)
            .IncrementsBy(1)
            .HasMin(100000)
            .HasMax(999999);

        var product = modelBuilder.Entity<Product>();
        product.ToTable("Products");
        product.HasKey(item => item.Id);
        product.Property(item => item.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEXT VALUE FOR ProductIds");
        product.Property(item => item.Name).HasMaxLength(200).IsRequired();
        product.Property(item => item.Stock).HasDefaultValue(0).IsRequired();
        product.Property(item => item.CreatedAt).IsRequired();
        product.Property(item => item.UpdatedAt);
        product.Property(item => item.Active).HasDefaultValue(true).IsRequired();

        ProductSeeder.Seed(modelBuilder);
    }
}
