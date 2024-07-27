using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWithOwnerReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerReview_items_ItemId",
                table: "OwnerReview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnerReview",
                table: "OwnerReview");

            migrationBuilder.RenameTable(
                name: "OwnerReview",
                newName: "OwnerReviews");

            migrationBuilder.RenameIndex(
                name: "IX_OwnerReview_ItemId",
                table: "OwnerReviews",
                newName: "IX_OwnerReviews_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnerReviews",
                table: "OwnerReviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerReviews_items_ItemId",
                table: "OwnerReviews",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerReviews_items_ItemId",
                table: "OwnerReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnerReviews",
                table: "OwnerReviews");

            migrationBuilder.RenameTable(
                name: "OwnerReviews",
                newName: "OwnerReview");

            migrationBuilder.RenameIndex(
                name: "IX_OwnerReviews_ItemId",
                table: "OwnerReview",
                newName: "IX_OwnerReview_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnerReview",
                table: "OwnerReview",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerReview_items_ItemId",
                table: "OwnerReview",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
