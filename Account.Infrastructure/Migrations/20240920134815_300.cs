using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _300 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "BaseUsers");

            migrationBuilder.DropColumn(
                name: "RegularUser_DeviceId",
                table: "BaseUsers");

            migrationBuilder.AddColumn<string>(
                name: "DeviceIds",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceIds",
                table: "BaseUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceIds",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "DeviceIds",
                table: "BaseUsers");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "Staffs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegularUser_DeviceId",
                table: "BaseUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
