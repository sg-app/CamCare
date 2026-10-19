using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddRepairOrderSerials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepairOrderSerials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepairOrderId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NeedsMaintenance = table.Column<bool>(type: "bit", nullable: false),
                    IsForeignCustomer = table.Column<bool>(type: "bit", nullable: false),
                    ArticleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerCustomerNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerCustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairOrderSerials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepairOrderSerials_RepairOrders_RepairOrderId",
                        column: x => x.RepairOrderId,
                        principalTable: "RepairOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrderSerials_RepairOrderId",
                table: "RepairOrderSerials",
                column: "RepairOrderId");

            // Bestehende Hauptseriennummern in die neue Tabelle übernehmen (Backfill), damit
            // die Ermittlung offener Aufträge und die Doppelauftrag-Prüfung auch für
            // Bestandsaufträge über die neue Tabelle funktionieren.
            migrationBuilder.Sql(
                "INSERT INTO RepairOrderSerials (RepairOrderId, SerialNumber, NeedsMaintenance, IsForeignCustomer, IsPrimary, CreatedAt) " +
                "SELECT ro.Id, LTRIM(RTRIM(ro.SerialNumber)), ro.SerialNumberNeedsMaintenance, 0, 1, SYSUTCDATETIME() " +
                "FROM RepairOrders ro " +
                "WHERE ro.SerialNumber IS NOT NULL AND LTRIM(RTRIM(ro.SerialNumber)) <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepairOrderSerials");
        }
    }
}
