using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_catalog_items",
                table: "catalog_items");

            migrationBuilder.RenameTable(
                name: "catalog_items",
                newName: "items");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "items",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "ItemCategoryId",
                table: "items",
                type: "nvarchar(26)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_items",
                table: "items",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_items_ItemCategoryId",
                table: "items",
                column: "ItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_categories_ItemCategoryId",
                table: "items",
                column: "ItemCategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_categories_ItemCategoryId",
                table: "items");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_items",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_ItemCategoryId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ItemCategoryId",
                table: "items");

            migrationBuilder.RenameTable(
                name: "items",
                newName: "catalog_items");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "catalog_items",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AddPrimaryKey(
                name: "PK_catalog_items",
                table: "catalog_items",
                column: "Id");
        }
    }
}
