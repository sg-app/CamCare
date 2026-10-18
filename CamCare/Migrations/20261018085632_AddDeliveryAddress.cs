using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerDescription",
                table: "RepairOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryAddressId",
                table: "RepairOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAddresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrders_DeliveryAddressId",
                table: "RepairOrders",
                column: "DeliveryAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrders_DeliveryAddresses_DeliveryAddressId",
                table: "RepairOrders",
                column: "DeliveryAddressId",
                principalTable: "DeliveryAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrders_DeliveryAddresses_DeliveryAddressId",
                table: "RepairOrders");

            migrationBuilder.DropTable(
                name: "DeliveryAddresses");

            migrationBuilder.DropIndex(
                name: "IX_RepairOrders_DeliveryAddressId",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "CustomerDescription",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddressId",
                table: "RepairOrders");
        }
    }
}
