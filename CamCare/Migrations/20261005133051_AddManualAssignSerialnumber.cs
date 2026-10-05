using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddManualAssignSerialnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCamera",
                table: "RepairOrders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SerialNumberNeedsMaintenance",
                table: "RepairOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCamera",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "SerialNumberNeedsMaintenance",
                table: "RepairOrders");
        }
    }
}
