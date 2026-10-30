using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderWork_Works_WorksId",
                table: "RepairOrderWork");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "RepairOrderWork",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "DefectiveRepairOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderWork_Works_WorksId",
                table: "RepairOrderWork",
                column: "WorksId",
                principalTable: "Works",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderWork_Works_WorksId",
                table: "RepairOrderWork");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "RepairOrderWork");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "DefectiveRepairOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderWork_Works_WorksId",
                table: "RepairOrderWork",
                column: "WorksId",
                principalTable: "Works",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
