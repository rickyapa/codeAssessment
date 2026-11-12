using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAssessment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "ProductIds",
                startValue: 100004L,
                minValue: 100000L,
                maxValue: 999999L);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR ProductIds",
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "ProductIds");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR ProductIds");
        }
    }
}
