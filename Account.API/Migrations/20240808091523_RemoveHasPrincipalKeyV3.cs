using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasPrincipalKeyV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NationalIdentities_Users_UserId",
                table: "NationalIdentities");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Users_BusinessUserId",
                table: "Staffs");

            migrationBuilder.AddForeignKey(
                name: "FK_NationalIdentities_Users_UserId",
                table: "NationalIdentities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Users_BusinessUserId",
                table: "Staffs",
                column: "BusinessUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NationalIdentities_Users_UserId",
                table: "NationalIdentities");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Users_BusinessUserId",
                table: "Staffs");

            migrationBuilder.AddForeignKey(
                name: "FK_NationalIdentities_Users_UserId",
                table: "NationalIdentities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Users_BusinessUserId",
                table: "Staffs",
                column: "BusinessUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
