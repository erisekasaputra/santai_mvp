using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWithOwnerReviewWithConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerReviews_items_ItemId",
                table: "OwnerReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnerReviews",
                table: "OwnerReviews");

            migrationBuilder.RenameTable(
                name: "OwnerReviews",
                newName: "owner_reviews");

            migrationBuilder.RenameIndex(
                name: "IX_OwnerReviews_ItemId",
                table: "owner_reviews",
                newName: "IX_owner_reviews_ItemId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "owner_reviews",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "owner_reviews",
                type: "nvarchar(26)",
                maxLength: 26,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_owner_reviews",
                table: "owner_reviews",
                column: "Id");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_owner_reviews",
                table: "owner_reviews");

            migrationBuilder.RenameTable(
                name: "owner_reviews",
                newName: "OwnerReviews");

            migrationBuilder.RenameIndex(
                name: "IX_owner_reviews_ItemId",
                table: "OwnerReviews",
                newName: "IX_OwnerReviews_ItemId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "OwnerReviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OwnerReviews",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(26)",
                oldMaxLength: 26);

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
    }
}
