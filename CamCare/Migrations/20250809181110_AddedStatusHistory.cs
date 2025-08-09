using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepairOrderStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepairOrderId = table.Column<int>(type: "int", nullable: false),
                    RepairOrderStatusId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairOrderStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepairOrderStatusHistories_RepairOrderStatuses_RepairOrderStatusId",
                        column: x => x.RepairOrderStatusId,
                        principalTable: "RepairOrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RepairOrderStatusHistories_RepairOrders_RepairOrderId",
                        column: x => x.RepairOrderId,
                        principalTable: "RepairOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrderStatusHistories_RepairOrderId",
                table: "RepairOrderStatusHistories",
                column: "RepairOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrderStatusHistories_RepairOrderStatusId",
                table: "RepairOrderStatusHistories",
                column: "RepairOrderStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepairOrderStatusHistories");
        }
    }
}
