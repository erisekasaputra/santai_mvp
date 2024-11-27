using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V100111000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceIds",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "DeviceIds",
                table: "BaseUsers");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "BaseUsers");

            migrationBuilder.AddColumn<string>(
                name: "Ratings",
                table: "BaseUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ratings",
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

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "BaseUsers",
                type: "decimal(5,2)",
                nullable: true);
        }
    }
}
