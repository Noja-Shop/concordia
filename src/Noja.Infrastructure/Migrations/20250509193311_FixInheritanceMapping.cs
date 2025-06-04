using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixInheritanceMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "AspNetUsers",
                newName: "UserTypeName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserTypeName",
                table: "AspNetUsers",
                newName: "Discriminator");
        }
    }
}
