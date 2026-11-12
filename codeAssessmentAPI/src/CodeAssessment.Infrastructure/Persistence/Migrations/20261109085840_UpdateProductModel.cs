using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAssessment.Infrastructure.Persistence.Migrations;

public partial class UpdateProductModel : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Products");

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Stock = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "Products",
            columns: new[] { "Id", "Active", "CreatedAt", "Name", "Stock", "UpdatedAt" },
            values: new object[,]
            {
                { 100001, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Product 1", 10, null },
                { 100002, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Product 2", 1, null },
                { 100003, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Product 3", 0, null }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Products");

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "Products",
            columns: new[] { "Id", "CreatedUtc", "Description", "Name", "Price", "UpdatedUtc" },
            values: new object[]
            {
                new Guid("d1b3d8d1-1b2e-4e8f-aad9-3d4c7b1c2a10"),
                new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                "A product created by the initial database seed.",
                "Sample product",
                9.99m,
                null
            });
    }
}
