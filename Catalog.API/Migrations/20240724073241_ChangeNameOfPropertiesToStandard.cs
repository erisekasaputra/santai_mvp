using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameOfPropertiesToStandard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_categories_ItemCategoryId",
                table: "items");

            migrationBuilder.RenameColumn(
                name: "ItemCategoryId",
                table: "items",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_items_ItemCategoryId",
                table: "items",
                newName: "IX_items_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_categories_CategoryId",
                table: "items",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_categories_CategoryId",
                table: "items");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "items",
                newName: "ItemCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_items_CategoryId",
                table: "items",
                newName: "IX_items_ItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_categories_ItemCategoryId",
                table: "items",
                column: "ItemCategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
