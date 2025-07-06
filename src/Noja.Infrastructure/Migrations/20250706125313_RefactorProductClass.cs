using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProductClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitSize",
                table: "Products",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "UnitOfMeasure",
                table: "Products",
                newName: "PackageType");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "PackagePrice");

            migrationBuilder.AddColumn<int>(
                name: "MeasurementUnit",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PackageSize",
                table: "Products",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasurementUnit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PackageSize",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Products",
                newName: "UnitSize");

            migrationBuilder.RenameColumn(
                name: "PackageType",
                table: "Products",
                newName: "UnitOfMeasure");

            migrationBuilder.RenameColumn(
                name: "PackagePrice",
                table: "Products",
                newName: "Price");
        }
    }
}
