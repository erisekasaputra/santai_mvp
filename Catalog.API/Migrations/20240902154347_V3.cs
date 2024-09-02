using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_owner_reviews_items_ItemId",
                table: "owner_reviews",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");
        }
    }
}
