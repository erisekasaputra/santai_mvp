using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.API.Migrations
{
    /// <inheritdoc />
    public partial class V100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staffs_HashedEmail",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_HashedPhoneNumber",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_BaseUsers_HashedEmail",
                table: "BaseUsers");

            migrationBuilder.DropIndex(
                name: "IX_BaseUsers_HashedPhoneNumber",
                table: "BaseUsers");

            migrationBuilder.AlterColumn<string>(
                name: "HashedPhoneNumber",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "HashedEmail",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedPhoneNumber",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedEmail",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "HashedPhoneNumber",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "HashedEmail",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedPhoneNumber",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedEmail",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_HashedEmail",
                table: "Staffs",
                column: "HashedEmail",
                unique: true,
                filter: "[HashedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_HashedPhoneNumber",
                table: "Staffs",
                column: "HashedPhoneNumber",
                unique: true,
                filter: "[HashedPhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUsers_HashedEmail",
                table: "BaseUsers",
                column: "HashedEmail",
                unique: true,
                filter: "[HashedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUsers_HashedPhoneNumber",
                table: "BaseUsers",
                column: "HashedPhoneNumber",
                unique: true,
                filter: "[HashedPhoneNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staffs_HashedEmail",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_HashedPhoneNumber",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_BaseUsers_HashedEmail",
                table: "BaseUsers");

            migrationBuilder.DropIndex(
                name: "IX_BaseUsers_HashedPhoneNumber",
                table: "BaseUsers");

            migrationBuilder.AlterColumn<string>(
                name: "HashedPhoneNumber",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HashedEmail",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedPhoneNumber",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedEmail",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HashedPhoneNumber",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HashedEmail",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedPhoneNumber",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedEmail",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_HashedEmail",
                table: "Staffs",
                column: "HashedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_HashedPhoneNumber",
                table: "Staffs",
                column: "HashedPhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaseUsers_HashedEmail",
                table: "BaseUsers",
                column: "HashedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaseUsers_HashedPhoneNumber",
                table: "BaseUsers",
                column: "HashedPhoneNumber",
                unique: true);
        }
    }
}
