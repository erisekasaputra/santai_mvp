using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V60 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MechanicOrderTasks");

            migrationBuilder.DropTable(
                name: "OrderTaskWaitingMechanicAssigns");

            migrationBuilder.DropTable(
                name: "OrderTaskWaitingMechanicConfirms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MechanicOrderTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsOrderAssigned = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float(24)", nullable: false),
                    Longitude = table.Column<double>(type: "float(24)", nullable: false),
                    MechanicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MechanicOrderTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MechanicOrderTasks_BaseUsers_MechanicId",
                        column: x => x.MechanicId,
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTaskWaitingMechanicAssigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAcceptedByMechanic = table.Column<bool>(type: "bit", nullable: false),
                    IsMechanicAssigned = table.Column<bool>(type: "bit", nullable: false),
                    IsOrderCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float(24)", nullable: false),
                    Longitude = table.Column<double>(type: "float(24)", nullable: false),
                    MechanicConfirmationExpire = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MechanicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RetryAttemp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTaskWaitingMechanicAssigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderTaskWaitingMechanicConfirms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsExpiryProcessed = table.Column<bool>(type: "bit", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    MechanicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTaskWaitingMechanicConfirms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MechanicOrderTasks_MechanicId",
                table: "MechanicOrderTasks",
                column: "MechanicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MechanicOrderTasks_OrderId",
                table: "MechanicOrderTasks",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaskWaitingMechanicAssigns_OrderId",
                table: "OrderTaskWaitingMechanicAssigns",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaskWaitingMechanicConfirms_OrderId",
                table: "OrderTaskWaitingMechanicConfirms",
                column: "OrderId",
                unique: true);
        }
    }
}
