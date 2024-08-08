using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class NewInitialUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Code",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_NationalIdentities_UserId_VerificationStatus",
                table: "NationalIdentities");

            migrationBuilder.DropIndex(
                name: "IX_DrivingLicenses_UserId_VerificationStatus",
                table: "DrivingLicenses");

            migrationBuilder.DropIndex(
                name: "IX_BusinessLicenses_LicenseNumber",
                table: "BusinessLicenses");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VerificationStatus",
                table: "BusinessLicenses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BusinessLicenses",
                type: "varchar(max)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Code",
                table: "Users",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIdentities_IdentityNumber_VerificationStatus",
                table: "NationalIdentities",
                columns: new[] { "IdentityNumber", "VerificationStatus" },
                unique: true,
                filter: "[VerificationStatus] =  'Accepted' ");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIdentities_UserId",
                table: "NationalIdentities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DrivingLicenses_LicenseNumber_VerificationStatus",
                table: "DrivingLicenses",
                columns: new[] { "LicenseNumber", "VerificationStatus" },
                unique: true,
                filter: "[VerificationStatus] = 'Accepted' ");

            migrationBuilder.CreateIndex(
                name: "IX_DrivingLicenses_UserId",
                table: "DrivingLicenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLicenses_LicenseNumber_VerificationStatus",
                table: "BusinessLicenses",
                columns: new[] { "LicenseNumber", "VerificationStatus" },
                unique: true,
                filter: " [VerificationStatus] = 'Accepted' ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Code",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_NationalIdentities_IdentityNumber_VerificationStatus",
                table: "NationalIdentities");

            migrationBuilder.DropIndex(
                name: "IX_NationalIdentities_UserId",
                table: "NationalIdentities");

            migrationBuilder.DropIndex(
                name: "IX_DrivingLicenses_LicenseNumber_VerificationStatus",
                table: "DrivingLicenses");

            migrationBuilder.DropIndex(
                name: "IX_DrivingLicenses_UserId",
                table: "DrivingLicenses");

            migrationBuilder.DropIndex(
                name: "IX_BusinessLicenses_LicenseNumber_VerificationStatus",
                table: "BusinessLicenses");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Users",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VerificationStatus",
                table: "BusinessLicenses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BusinessLicenses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldMaxLength: 1000);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Code",
                table: "Users",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIdentities_UserId_VerificationStatus",
                table: "NationalIdentities",
                columns: new[] { "UserId", "VerificationStatus" },
                unique: true,
                filter: "[VerificationStatus] =  'Accepted' ");

            migrationBuilder.CreateIndex(
                name: "IX_DrivingLicenses_UserId_VerificationStatus",
                table: "DrivingLicenses",
                columns: new[] { "UserId", "VerificationStatus" },
                unique: true,
                filter: "[VerificationStatus] = 'Accepted' ");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLicenses_LicenseNumber",
                table: "BusinessLicenses",
                column: "LicenseNumber",
                unique: true);
        }
    }
}
