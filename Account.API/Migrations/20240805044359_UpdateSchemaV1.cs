using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Users_MechanicUserId",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_DrivingLicenses_Users_UserId",
                table: "DrivingLicenses");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyPrograms_Users_LoyaltyUserId",
                table: "LoyaltyPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferralPrograms_Users_UserId",
                table: "ReferralPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferredPrograms_Users_ReferrerId",
                table: "ReferredPrograms");

            migrationBuilder.DropColumn(
                name: "ValidDate",
                table: "Certifications");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "ReferredDate",
                table: "ReferredPrograms",
                newName: "ReferredDateUtc");

            migrationBuilder.RenameColumn(
                name: "ReferralDate",
                table: "ReferralPrograms",
                newName: "ValidDateUtc");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidDateUtc",
                table: "Certifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Users_MechanicUserId",
                table: "Certifications",
                column: "MechanicUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DrivingLicenses_Users_UserId",
                table: "DrivingLicenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyPrograms_Users_LoyaltyUserId",
                table: "LoyaltyPrograms",
                column: "LoyaltyUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralPrograms_Users_UserId",
                table: "ReferralPrograms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferredPrograms_Users_ReferrerId",
                table: "ReferredPrograms",
                column: "ReferrerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Users_MechanicUserId",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_DrivingLicenses_Users_UserId",
                table: "DrivingLicenses");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyPrograms_Users_LoyaltyUserId",
                table: "LoyaltyPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferralPrograms_Users_UserId",
                table: "ReferralPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferredPrograms_Users_ReferrerId",
                table: "ReferredPrograms");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "ValidDateUtc",
                table: "Certifications");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ReferredDateUtc",
                table: "ReferredPrograms",
                newName: "ReferredDate");

            migrationBuilder.RenameColumn(
                name: "ValidDateUtc",
                table: "ReferralPrograms",
                newName: "ReferralDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidDate",
                table: "Certifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Users_MechanicUserId",
                table: "Certifications",
                column: "MechanicUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DrivingLicenses_Users_UserId",
                table: "DrivingLicenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyPrograms_Users_LoyaltyUserId",
                table: "LoyaltyPrograms",
                column: "LoyaltyUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralPrograms_Users_UserId",
                table: "ReferralPrograms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferredPrograms_Users_ReferrerId",
                table: "ReferredPrograms",
                column: "ReferrerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
