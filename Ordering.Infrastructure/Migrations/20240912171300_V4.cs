using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buyers_Orders_OrderingId",
                table: "Buyers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cancellations_Orders_OrderingId",
                table: "Cancellations");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Orders_OrderingId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Fees_Orders_OrderingId",
                table: "Fees");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Orders_OrderingId",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_OrderingId",
                table: "LineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Mechanics_Orders_OrderingId",
                table: "Mechanics");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderingId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "Payments",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderingId",
                table: "Payments",
                newName: "IX_Payments_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "Mechanics",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Mechanics_OrderingId",
                table: "Mechanics",
                newName: "IX_Mechanics_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "LineItems",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_LineItems_OrderingId",
                table: "LineItems",
                newName: "IX_LineItems_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "Fleets",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Fleets_OrderingId",
                table: "Fleets",
                newName: "IX_Fleets_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "Fees",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Fees_OrderingId",
                table: "Fees",
                newName: "IX_Fees_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "Coupons",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Coupons_OrderingId",
                table: "Coupons",
                newName: "IX_Coupons_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "Cancellations",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Cancellations_OrderingId",
                table: "Cancellations",
                newName: "IX_Cancellations_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderingId",
                table: "Buyers",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Buyers_OrderingId",
                table: "Buyers",
                newName: "IX_Buyers_OrderId");

            migrationBuilder.AddColumn<Guid>(
                name: "LineItemId",
                table: "LineItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FleetId",
                table: "Fleets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_Buyers_Orders_OrderId",
                table: "Buyers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cancellations_Orders_OrderId",
                table: "Cancellations",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Orders_OrderId",
                table: "Coupons",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_Orders_OrderId",
                table: "Fees",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_Orders_OrderId",
                table: "Fleets",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mechanics_Orders_OrderId",
                table: "Mechanics",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buyers_Orders_OrderId",
                table: "Buyers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cancellations_Orders_OrderId",
                table: "Cancellations");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Orders_OrderId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Fees_Orders_OrderId",
                table: "Fees");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Orders_OrderId",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Mechanics_Orders_OrderId",
                table: "Mechanics");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LineItemId",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "FleetId",
                table: "Fleets");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Payments",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                newName: "IX_Payments_OrderingId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Mechanics",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_Mechanics_OrderId",
                table: "Mechanics",
                newName: "IX_Mechanics_OrderingId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "LineItems",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems",
                newName: "IX_LineItems_OrderingId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Fleets",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_Fleets_OrderId",
                table: "Fleets",
                newName: "IX_Fleets_OrderingId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Fees",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_Fees_OrderId",
                table: "Fees",
                newName: "IX_Fees_OrderingId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Coupons",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_Coupons_OrderId",
                table: "Coupons",
                newName: "IX_Coupons_OrderingId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Cancellations",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_Cancellations_OrderId",
                table: "Cancellations",
                newName: "IX_Cancellations_OrderingId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Buyers",
                newName: "OrderingId");

            migrationBuilder.RenameIndex(
                name: "IX_Buyers_OrderId",
                table: "Buyers",
                newName: "IX_Buyers_OrderingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buyers_Orders_OrderingId",
                table: "Buyers",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cancellations_Orders_OrderingId",
                table: "Cancellations",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Orders_OrderingId",
                table: "Coupons",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_Orders_OrderingId",
                table: "Fees",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_Orders_OrderingId",
                table: "Fleets",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_OrderingId",
                table: "LineItems",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mechanics_Orders_OrderingId",
                table: "Mechanics",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderingId",
                table: "Payments",
                column: "OrderingId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
