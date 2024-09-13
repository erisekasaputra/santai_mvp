using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MechanicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Latitude = table.Column<double>(type: "float(24)", nullable: false),
                    Longitude = table.Column<double>(type: "float(24)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTasks_BaseUsers_MechanicId",
                        column: x => x.MechanicId,
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderTasks_MechanicId",
                table: "OrderTasks",
                column: "MechanicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTasks_OrderId",
                table: "OrderTasks",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderTasks");
        }
    }
}
