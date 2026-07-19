using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPI2026Demo.Migrations
{
    /// <inheritdoc />
    public partial class _003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Catalog",
                table: "Artifacts",
                newName: "CatalogNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CatalogNumber",
                table: "Artifacts",
                newName: "Catalog");
        }
    }
}
