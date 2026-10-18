using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentStateAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentStateAt",
                table: "RepairOrders",
                type: "datetime2",
                nullable: true);

            // Bestehende Aufträge mit dem Zeitpunkt der letzten Statusänderung befüllen.
            // Existiert keine Historie, wird der Erstellzeitpunkt verwendet.
            migrationBuilder.Sql(
                """
                UPDATE ro
                SET ro.CurrentStateAt = ISNULL(h.ChangedAt, ro.CreatedAt)
                FROM RepairOrders ro
                OUTER APPLY (
                    SELECT TOP 1 hs.ChangedAt
                    FROM RepairOrderStatusHistories hs
                    WHERE hs.RepairOrderId = ro.Id
                    ORDER BY hs.ChangedAt DESC, hs.Id DESC
                ) h
                WHERE ro.CurrentStateAt IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStateAt",
                table: "RepairOrders");
        }
    }
}
