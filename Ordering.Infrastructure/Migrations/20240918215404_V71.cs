using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V71 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InspectionStatus",
                table: "Fleets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "FeeDescription",
                table: "Fees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FeeDescription",
                table: "CancellationFee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "BasicInspection",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FleetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FleetAggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    EntityStateAction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasicInspection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasicInspection_Fleets_FleetAggregateId",
                        column: x => x.FleetAggregateId,
                        principalTable: "Fleets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreServiceInspection",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FleetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FleetAggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    EntityStateAction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreServiceInspection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreServiceInspection_Fleets_FleetAggregateId",
                        column: x => x.FleetAggregateId,
                        principalTable: "Fleets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreServiceInspectionResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FleetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreServiceInspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsWorking = table.Column<bool>(type: "bit", nullable: false),
                    EntityStateAction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreServiceInspectionResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreServiceInspectionResult_PreServiceInspection_PreServiceInspectionId",
                        column: x => x.PreServiceInspectionId,
                        principalTable: "PreServiceInspection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasicInspection_FleetAggregateId",
                table: "BasicInspection",
                column: "FleetAggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_PreServiceInspection_FleetAggregateId",
                table: "PreServiceInspection",
                column: "FleetAggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_PreServiceInspectionResult_PreServiceInspectionId",
                table: "PreServiceInspectionResult",
                column: "PreServiceInspectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasicInspection");

            migrationBuilder.DropTable(
                name: "PreServiceInspectionResult");

            migrationBuilder.DropTable(
                name: "PreServiceInspection");

            migrationBuilder.DropColumn(
                name: "InspectionStatus",
                table: "Fleets");

            migrationBuilder.AlterColumn<string>(
                name: "FeeDescription",
                table: "Fees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FeeDescription",
                table: "CancellationFee",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
