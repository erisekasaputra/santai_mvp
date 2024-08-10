using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdentityId",
                table: "Staffs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_IdentityId",
                table: "Staffs",
                column: "IdentityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staffs_IdentityId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "Staffs");
        }
    }
}
