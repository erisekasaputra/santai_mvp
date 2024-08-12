using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class V13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaxId",
                table: "Users",
                newName: "NewHashedPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Users",
                newName: "HashedPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "NewPhoneNumber",
                table: "Users",
                newName: "NewHashedEmail");

            migrationBuilder.RenameColumn(
                name: "NewEmail",
                table: "Users",
                newName: "NewEncryptedPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "HashedEmail");

            migrationBuilder.RenameColumn(
                name: "ContactPerson",
                table: "Users",
                newName: "NewEncryptedEmail");

            migrationBuilder.RenameColumn(
                name: "Address_AddressLine3",
                table: "Users",
                newName: "EncryptedTaxId");

            migrationBuilder.RenameColumn(
                name: "Address_AddressLine2",
                table: "Users",
                newName: "EncryptedContactPerson");

            migrationBuilder.RenameColumn(
                name: "Address_AddressLine1",
                table: "Users",
                newName: "EncryptedPhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                newName: "IX_Users_HashedPhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Users_HashedEmail");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Staffs",
                newName: "HashedPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "NewPhoneNumber",
                table: "Staffs",
                newName: "NewHashedPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "NewEmail",
                table: "Staffs",
                newName: "NewHashedEmail");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Staffs",
                newName: "HashedEmail");

            migrationBuilder.RenameColumn(
                name: "Address_AddressLine3",
                table: "Staffs",
                newName: "NewEncryptedPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Address_AddressLine2",
                table: "Staffs",
                newName: "NewEncryptedEmail");

            migrationBuilder.RenameColumn(
                name: "Address_AddressLine1",
                table: "Staffs",
                newName: "EncryptedPhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_PhoneNumber",
                table: "Staffs",
                newName: "IX_Staffs_HashedPhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_Email",
                table: "Staffs",
                newName: "IX_Staffs_HashedEmail");

            migrationBuilder.RenameColumn(
                name: "IdentityNumber",
                table: "NationalIdentities",
                newName: "HashedIdentityNumber");

            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "DrivingLicenses",
                newName: "HashedLicenseNumber");

            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "BusinessLicenses",
                newName: "HashedLicenseNumber");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessLicenses_LicenseNumber_VerificationStatus",
                table: "BusinessLicenses",
                newName: "IX_BusinessLicenses_HashedLicenseNumber_VerificationStatus");

            migrationBuilder.AddColumn<string>(
                name: "Address_EncryptedAddressLine1",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_EncryptedAddressLine2",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_EncryptedAddressLine3",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EncryptedEmail",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_EncryptedAddressLine1",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_EncryptedAddressLine2",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_EncryptedAddressLine3",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EncryptedEmail",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedIdentityNumber",
                table: "NationalIdentities",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedLicenseNumber",
                table: "DrivingLicenses",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedLicenseNumber",
                table: "BusinessLicenses",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_EncryptedAddressLine1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address_EncryptedAddressLine2",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address_EncryptedAddressLine3",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EncryptedEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address_EncryptedAddressLine1",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Address_EncryptedAddressLine2",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Address_EncryptedAddressLine3",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "EncryptedEmail",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "EncryptedIdentityNumber",
                table: "NationalIdentities");

            migrationBuilder.DropColumn(
                name: "EncryptedLicenseNumber",
                table: "DrivingLicenses");

            migrationBuilder.DropColumn(
                name: "EncryptedLicenseNumber",
                table: "BusinessLicenses");

            migrationBuilder.RenameColumn(
                name: "NewHashedPhoneNumber",
                table: "Users",
                newName: "TaxId");

            migrationBuilder.RenameColumn(
                name: "NewHashedEmail",
                table: "Users",
                newName: "NewPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "NewEncryptedPhoneNumber",
                table: "Users",
                newName: "NewEmail");

            migrationBuilder.RenameColumn(
                name: "NewEncryptedEmail",
                table: "Users",
                newName: "ContactPerson");

            migrationBuilder.RenameColumn(
                name: "HashedPhoneNumber",
                table: "Users",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "HashedEmail",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "EncryptedTaxId",
                table: "Users",
                newName: "Address_AddressLine3");

            migrationBuilder.RenameColumn(
                name: "EncryptedPhoneNumber",
                table: "Users",
                newName: "Address_AddressLine1");

            migrationBuilder.RenameColumn(
                name: "EncryptedContactPerson",
                table: "Users",
                newName: "Address_AddressLine2");

            migrationBuilder.RenameIndex(
                name: "IX_Users_HashedPhoneNumber",
                table: "Users",
                newName: "IX_Users_PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Users_HashedEmail",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "NewHashedPhoneNumber",
                table: "Staffs",
                newName: "NewPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "NewHashedEmail",
                table: "Staffs",
                newName: "NewEmail");

            migrationBuilder.RenameColumn(
                name: "NewEncryptedPhoneNumber",
                table: "Staffs",
                newName: "Address_AddressLine3");

            migrationBuilder.RenameColumn(
                name: "NewEncryptedEmail",
                table: "Staffs",
                newName: "Address_AddressLine2");

            migrationBuilder.RenameColumn(
                name: "HashedPhoneNumber",
                table: "Staffs",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "HashedEmail",
                table: "Staffs",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "EncryptedPhoneNumber",
                table: "Staffs",
                newName: "Address_AddressLine1");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_HashedPhoneNumber",
                table: "Staffs",
                newName: "IX_Staffs_PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_HashedEmail",
                table: "Staffs",
                newName: "IX_Staffs_Email");

            migrationBuilder.RenameColumn(
                name: "HashedIdentityNumber",
                table: "NationalIdentities",
                newName: "IdentityNumber");

            migrationBuilder.RenameColumn(
                name: "HashedLicenseNumber",
                table: "DrivingLicenses",
                newName: "LicenseNumber");

            migrationBuilder.RenameColumn(
                name: "HashedLicenseNumber",
                table: "BusinessLicenses",
                newName: "LicenseNumber");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessLicenses_HashedLicenseNumber_VerificationStatus",
                table: "BusinessLicenses",
                newName: "IX_BusinessLicenses_LicenseNumber_VerificationStatus");
        }
    }
}
