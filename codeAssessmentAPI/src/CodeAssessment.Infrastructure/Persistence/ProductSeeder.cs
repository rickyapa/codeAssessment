using CodeAssessment.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodeAssessment.Infrastructure.Persistence;

internal static class ProductSeeder
{
    private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new
            {
                Id = 100001,
                Name = "Product 1",
                Stock = 10,
                CreatedAt = SeedCreatedAt,
                UpdatedAt = (DateTime?)null,
                Active = true
            },
            new
            {
                Id = 100002,
                Name = "Product 2",
                Stock = 1,
                CreatedAt = SeedCreatedAt,
                UpdatedAt = (DateTime?)null,
                Active = true
            },
            new
            {
                Id = 100003,
                Name = "Product 3",
                Stock = 0,
                CreatedAt = SeedCreatedAt,
                UpdatedAt = (DateTime?)null,
                Active = true
            });
    }
}
