using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddedIncludedComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalComponents",
                table: "RepairOrders");

            migrationBuilder.CreateTable(
                name: "IncludedComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncludedComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncludedComponentRepairOrder",
                columns: table => new
                {
                    IncludedComponentsId = table.Column<int>(type: "int", nullable: false),
                    RepairOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncludedComponentRepairOrder", x => new { x.IncludedComponentsId, x.RepairOrdersId });
                    table.ForeignKey(
                        name: "FK_IncludedComponentRepairOrder_IncludedComponents_IncludedComponentsId",
                        column: x => x.IncludedComponentsId,
                        principalTable: "IncludedComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncludedComponentRepairOrder_RepairOrders_RepairOrdersId",
                        column: x => x.RepairOrdersId,
                        principalTable: "RepairOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncludedComponentRepairOrder_RepairOrdersId",
                table: "IncludedComponentRepairOrder",
                column: "RepairOrdersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncludedComponentRepairOrder");

            migrationBuilder.DropTable(
                name: "IncludedComponents");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalComponents",
                table: "RepairOrders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
