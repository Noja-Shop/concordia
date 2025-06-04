using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicatePhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SellerPhoneNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellerPhoneNumber",
                table: "AspNetUsers");
        }
    }
}
