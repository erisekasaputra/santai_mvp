using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasPrincipalKeyV4 : Migration
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
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Users_BusinessUserId",
                table: "Staffs",
                column: "BusinessUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
    }
}
