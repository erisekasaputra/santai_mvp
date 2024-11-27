using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V2802948723042 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalCancelledJob",
                table: "BaseUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCompletedJob",
                table: "BaseUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalEntireJob",
                table: "BaseUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalEntireJobBothCompleteIncomplete",
                table: "BaseUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCancelledJob",
                table: "BaseUsers");

            migrationBuilder.DropColumn(
                name: "TotalCompletedJob",
                table: "BaseUsers");

            migrationBuilder.DropColumn(
                name: "TotalEntireJob",
                table: "BaseUsers");

            migrationBuilder.DropColumn(
                name: "TotalEntireJobBothCompleteIncomplete",
                table: "BaseUsers");
        }
    }
}
