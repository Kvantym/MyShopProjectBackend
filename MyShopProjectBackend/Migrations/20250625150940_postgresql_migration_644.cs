using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShopProjectBackend.Migrations
{
    /// <inheritdoc />
    public partial class postgresql_migration_644 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shops_users_UserId",
                table: "shops");

            migrationBuilder.DropIndex(
                name: "IX_shops_UserId",
                table: "shops");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "shops");

            migrationBuilder.CreateIndex(
                name: "IX_shops_OwnerId",
                table: "shops",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_shops_users_OwnerId",
                table: "shops",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shops_users_OwnerId",
                table: "shops");

            migrationBuilder.DropIndex(
                name: "IX_shops_OwnerId",
                table: "shops");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "shops",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_shops_UserId",
                table: "shops",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_shops_users_UserId",
                table: "shops",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
