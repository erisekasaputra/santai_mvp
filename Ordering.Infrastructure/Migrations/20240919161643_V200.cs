using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V200 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityStateAction",
                table: "PreServiceInspectionResult");

            migrationBuilder.DropColumn(
                name: "EntityStateAction",
                table: "PreServiceInspection");

            migrationBuilder.DropColumn(
                name: "EntityStateAction",
                table: "BasicInspection");

            migrationBuilder.AddColumn<bool>(
                name: "IsRefundPaid",
                table: "Cancellations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShouldRefundAtUt",
                table: "Cancellations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Buyers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Buyers",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRefundPaid",
                table: "Cancellations");

            migrationBuilder.DropColumn(
                name: "ShouldRefundAtUt",
                table: "Cancellations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Buyers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Buyers");

            migrationBuilder.AddColumn<int>(
                name: "EntityStateAction",
                table: "PreServiceInspectionResult",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntityStateAction",
                table: "PreServiceInspection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntityStateAction",
                table: "BasicInspection",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
