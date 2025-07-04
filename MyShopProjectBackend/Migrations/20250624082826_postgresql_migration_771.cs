using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShopProjectBackend.Migrations
{
    /// <inheritdoc />
    public partial class postgresql_migration_771 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "products",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageMimeType",
                table: "products",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "products");

            migrationBuilder.DropColumn(
                name: "ImageMimeType",
                table: "products");
        }
    }
}
