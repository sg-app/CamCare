using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddWorks2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderWork_Work_WorksId",
                table: "RepairOrderWork");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Work",
                table: "Work");

            migrationBuilder.RenameTable(
                name: "Work",
                newName: "Works");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Works",
                table: "Works",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderWork_Works_WorksId",
                table: "RepairOrderWork",
                column: "WorksId",
                principalTable: "Works",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderWork_Works_WorksId",
                table: "RepairOrderWork");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Works",
                table: "Works");

            migrationBuilder.RenameTable(
                name: "Works",
                newName: "Work");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Work",
                table: "Work",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderWork_Work_WorksId",
                table: "RepairOrderWork",
                column: "WorksId",
                principalTable: "Work",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
