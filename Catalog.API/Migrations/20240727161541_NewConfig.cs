using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class NewConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews");

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "owner_reviews",
                type: "nvarchar(26)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(26)");

            migrationBuilder.AddForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews");

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "owner_reviews",
                type: "nvarchar(26)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(26)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
