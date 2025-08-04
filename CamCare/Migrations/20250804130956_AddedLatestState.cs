using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddedLatestState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOrderClose",
                table: "RepairOrderStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsDefault", "IsOrderClose" },
                values: new object[] { true, false });

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsOrderClose",
                value: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsOrderClose",
                value: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsOrderClose",
                value: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsOrderClose",
                value: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsOrderClose",
                value: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsOrderClose",
                value: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsOrderClose",
                value: false);

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsOrderClose",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOrderClose",
                table: "RepairOrderStatuses");

            migrationBuilder.UpdateData(
                table: "RepairOrderStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsDefault",
                value: false);
        }
    }
}
