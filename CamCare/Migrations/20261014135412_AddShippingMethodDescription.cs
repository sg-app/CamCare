using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingMethodDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingMethodDescription",
                table: "RepairOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingMethodDescription",
                table: "RepairOrders");
        }
    }
}
