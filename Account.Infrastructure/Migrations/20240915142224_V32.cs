using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_BaseUsers_UserId",
                table: "Fleets");

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_BaseUsers_UserId",
                table: "Fleets",
                column: "UserId",
                principalTable: "BaseUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_BaseUsers_UserId",
                table: "Fleets");

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_BaseUsers_UserId",
                table: "Fleets",
                column: "UserId",
                principalTable: "BaseUsers",
                principalColumn: "Id");
        }
    }
}
