using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NationalIdentities_UserId_VerificationStatus",
                table: "NationalIdentities");

            migrationBuilder.DropIndex(
                name: "IX_DrivingLicenses_UserId_VerificationStatus",
                table: "DrivingLicenses");

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
        }
    }
}
