using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class MakeRepairOrderStatusHistorySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrderStatusHistories");

            migrationBuilder.AlterColumn<int>(
                name: "RepairOrderStatusId",
                table: "RepairOrderStatusHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "RepairOrderStatusHistories",
                type: "nvarchar(30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontColor",
                table: "RepairOrderStatusHistories",
                type: "nvarchar(30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusName",
                table: "RepairOrderStatusHistories",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrderStatusHistories",
                column: "RepairOrderStatusId",
                principalTable: "RepairOrderStatuses",
                principalColumn: "Id");

            // Bestehende History-Einträge mit dem Statusnamen/-stil zum Zeitpunkt der Änderung befüllen (Backfill),
            // damit die Historie auch nach dem Löschen eines Status nachvollziehbar bleibt.
            migrationBuilder.Sql(
                """
                UPDATE h
                SET h.StatusName = s.Name,
                    h.BackgroundColor = s.BackgroundColor,
                    h.FontColor = s.FontColor
                FROM RepairOrderStatusHistories h
                INNER JOIN RepairOrderStatuses s ON h.RepairOrderStatusId = s.Id
                WHERE h.StatusName IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "RepairOrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "FontColor",
                table: "RepairOrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "StatusName",
                table: "RepairOrderStatusHistories");

            migrationBuilder.AlterColumn<int>(
                name: "RepairOrderStatusId",
                table: "RepairOrderStatusHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderStatusHistories_RepairOrderStatuses_RepairOrderStatusId",
                table: "RepairOrderStatusHistories",
                column: "RepairOrderStatusId",
                principalTable: "RepairOrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
