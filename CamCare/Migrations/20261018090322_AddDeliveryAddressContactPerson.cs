using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAddressContactPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Label",
                table: "DeliveryAddresses",
                newName: "ContactPerson");

            migrationBuilder.AddColumn<bool>(
                name: "PriorityRepair",
                table: "RepairOrders",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriorityRepair",
                table: "RepairOrders");

            migrationBuilder.RenameColumn(
                name: "ContactPerson",
                table: "DeliveryAddresses",
                newName: "Label");
        }
    }
}
