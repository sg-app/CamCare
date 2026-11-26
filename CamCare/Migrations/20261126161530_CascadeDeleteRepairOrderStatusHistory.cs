using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteRepairOrderStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrders_RepairOrderId",
                table: "RepairOrderStatusHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrders_RepairOrderId",
                table: "RepairOrderStatusHistories",
                column: "RepairOrderId",
                principalTable: "RepairOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrders_RepairOrderId",
                table: "RepairOrderStatusHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrders_RepairOrderId",
                table: "RepairOrderStatusHistories",
                column: "RepairOrderId",
                principalTable: "RepairOrders",
                principalColumn: "Id");
        }
    }
}
