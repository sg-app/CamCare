using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_CameraTypes_CameraTypeId",
                table: "Cameras");

            migrationBuilder.DropForeignKey(
                name: "FK_DefectiveRepairOrder_Defectives_DefectivesId",
                table: "DefectiveRepairOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRepairOrder_Employee_EmployeesId",
                table: "EmployeeRepairOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_IncludedComponentRepairOrder_IncludedComponents_IncludedComponentsId",
                table: "IncludedComponentRepairOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderRepairPosition_RepairPositions_RepairPositionId",
                table: "RepairOrderRepairPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrders_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrders");

            migrationBuilder.DropColumn(
                name: "RepairPostionId",
                table: "RepairOrderRepairPosition");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_CameraTypes_CameraTypeId",
                table: "Cameras",
                column: "CameraTypeId",
                principalTable: "CameraTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefectiveRepairOrder_Defectives_DefectivesId",
                table: "DefectiveRepairOrder",
                column: "DefectivesId",
                principalTable: "Defectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRepairOrder_Employee_EmployeesId",
                table: "EmployeeRepairOrder",
                column: "EmployeesId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncludedComponentRepairOrder_IncludedComponents_IncludedComponentsId",
                table: "IncludedComponentRepairOrder",
                column: "IncludedComponentsId",
                principalTable: "IncludedComponents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderRepairPosition_RepairPositions_RepairPositionId",
                table: "RepairOrderRepairPosition",
                column: "RepairPositionId",
                principalTable: "RepairPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrders_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrders",
                column: "RepairOrderStatusId",
                principalTable: "RepairOrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_CameraTypes_CameraTypeId",
                table: "Cameras");

            migrationBuilder.DropForeignKey(
                name: "FK_DefectiveRepairOrder_Defectives_DefectivesId",
                table: "DefectiveRepairOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRepairOrder_Employee_EmployeesId",
                table: "EmployeeRepairOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_IncludedComponentRepairOrder_IncludedComponents_IncludedComponentsId",
                table: "IncludedComponentRepairOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderRepairPosition_RepairPositions_RepairPositionId",
                table: "RepairOrderRepairPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrders_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrders");

            migrationBuilder.AddColumn<int>(
                name: "RepairPostionId",
                table: "RepairOrderRepairPosition",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_CameraTypes_CameraTypeId",
                table: "Cameras",
                column: "CameraTypeId",
                principalTable: "CameraTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefectiveRepairOrder_Defectives_DefectivesId",
                table: "DefectiveRepairOrder",
                column: "DefectivesId",
                principalTable: "Defectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRepairOrder_Employee_EmployeesId",
                table: "EmployeeRepairOrder",
                column: "EmployeesId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncludedComponentRepairOrder_IncludedComponents_IncludedComponentsId",
                table: "IncludedComponentRepairOrder",
                column: "IncludedComponentsId",
                principalTable: "IncludedComponents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderRepairPosition_RepairPositions_RepairPositionId",
                table: "RepairOrderRepairPosition",
                column: "RepairPositionId",
                principalTable: "RepairPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrders_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrders",
                column: "RepairOrderStatusId",
                principalTable: "RepairOrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
