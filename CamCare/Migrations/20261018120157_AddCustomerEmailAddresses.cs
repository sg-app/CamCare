using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerEmailAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailAddressRepairOrder",
                columns: table => new
                {
                    EmailAddressesId = table.Column<int>(type: "int", nullable: false),
                    RepairOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddressRepairOrder", x => new { x.EmailAddressesId, x.RepairOrdersId });
                    table.ForeignKey(
                        name: "FK_EmailAddressRepairOrder_EmailAddresses_EmailAddressesId",
                        column: x => x.EmailAddressesId,
                        principalTable: "EmailAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailAddressRepairOrder_RepairOrders_RepairOrdersId",
                        column: x => x.RepairOrdersId,
                        principalTable: "RepairOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailAddressRepairOrder_RepairOrdersId",
                table: "EmailAddressRepairOrder",
                column: "RepairOrdersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAddressRepairOrder");

            migrationBuilder.DropTable(
                name: "EmailAddresses");
        }
    }
}
