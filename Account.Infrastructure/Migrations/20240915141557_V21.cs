using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_BaseUsers_BaseUserId",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Staffs_StaffId",
                table: "Fleets");

            migrationBuilder.DropIndex(
                name: "IX_Fleets_BaseUserId",
                table: "Fleets");

            migrationBuilder.DropColumn(
                name: "BaseUserId",
                table: "Fleets");

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_UserId",
                table: "Fleets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_BaseUsers_UserId",
                table: "Fleets",
                column: "UserId",
                principalTable: "BaseUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_Staffs_StaffId",
                table: "Fleets",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_BaseUsers_UserId",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Staffs_StaffId",
                table: "Fleets");

            migrationBuilder.DropIndex(
                name: "IX_Fleets_UserId",
                table: "Fleets");

            migrationBuilder.AddColumn<Guid>(
                name: "BaseUserId",
                table: "Fleets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_BaseUserId",
                table: "Fleets",
                column: "BaseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_BaseUsers_BaseUserId",
                table: "Fleets",
                column: "BaseUserId",
                principalTable: "BaseUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_Staffs_StaffId",
                table: "Fleets",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
