using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddPackagingOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PackagingComment",
                table: "RepairOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackagingHeight",
                table: "RepairOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackagingLength",
                table: "RepairOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackagingType",
                table: "RepairOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackagingWidth",
                table: "RepairOrders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackagingComment",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "PackagingHeight",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "PackagingLength",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "PackagingType",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "PackagingWidth",
                table: "RepairOrders");
        }
    }
}
