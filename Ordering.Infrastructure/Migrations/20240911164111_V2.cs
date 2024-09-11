using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buyers_Orderings_OrderingId",
                table: "Buyers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cancellations_Orderings_OrderingId",
                table: "Cancellations");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Orderings_OrderingId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Fees_Orderings_OrderingId",
                table: "Fees");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Orderings_OrderingId",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orderings_OrderingId",
                table: "LineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Mechanics_Orderings_OrderingId",
                table: "Mechanics");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orderings_OrderingId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orderings",
                table: "Orderings");

            migrationBuilder.RenameTable(
                name: "Orderings",
                newName: "Orders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Orderings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orderings",
                table: "Orderings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Buyers_Orderings_OrderingId",
                table: "Buyers",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cancellations_Orderings_OrderingId",
                table: "Cancellations",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Orderings_OrderingId",
                table: "Coupons",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_Orderings_OrderingId",
                table: "Fees",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_Orderings_OrderingId",
                table: "Fleets",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orderings_OrderingId",
                table: "LineItems",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mechanics_Orderings_OrderingId",
                table: "Mechanics",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orderings_OrderingId",
                table: "Payments",
                column: "OrderingId",
                principalTable: "Orderings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
