using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddMailTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailSettings_RepairOrderStatuses_TriggerStatusId",
                table: "MailSettings");

            migrationBuilder.DropIndex(
                name: "IX_MailSettings_TriggerStatusId",
                table: "MailSettings");

            migrationBuilder.AddColumn<int>(
                name: "MailTemplateId",
                table: "RepairOrderMailLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepairOrderStatusId = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SubjectTemplate = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailTemplates_RepairOrderStatuses_RepairOrderStatusId",
                        column: x => x.RepairOrderStatusId,
                        principalTable: "RepairOrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MailTemplates",
                columns: new[] { "Id", "ArchivedAt", "BodyTemplate", "CreatedAt", "IsEnabled", "Order", "RepairOrderStatusId", "SubjectTemplate", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, "<p>Guten Tag {{CustomerName}},</p>\r\n<p>Ihr Gerät (Seriennummer {{SerialNumber}}) ist zu Ihrem Reparaturauftrag\r\n<strong>{{OrderNumber}}</strong> bei uns eingetroffen.</p>\r\n<p>Eingetroffen am: {{ArrivedAt}}</p>\r\n<p>Wir beginnen mit der Begutachtung und melden uns mit den nächsten Schritten.</p>\r\n<p>Mit freundlichen Grüßen<br />Ihr CamCare-Team</p>", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 2, "Ihr Gerät ist eingetroffen – Auftrag {{OrderNumber}}", null },
                    { 2, null, "<p>Guten Tag {{CustomerName}},</p>\r\n<p>Ihr Reparaturauftrag <strong>{{OrderNumber}}</strong> (Seriennummer {{SerialNumber}})\r\nwurde an Sie versendet.</p>\r\n<p>Lieferanschrift:<br />{{DeliveryAddressRecipient}}<br />{{DeliveryAddressStreet}}<br />{{DeliveryAddressCity}}</p>\r\n<p>Stand: {{CurrentStateAt}}</p>\r\n<p>Mit freundlichen Grüßen<br />Ihr CamCare-Team</p>", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 9, "Ihr Auftrag {{OrderNumber}} wurde versendet – {{Status}}", null }
                });

            // Datenübernahme: Die bisher in den MailSettings hinterlegte Einzelvorlage
            // (Status + Betreff + Inhalt) wird als Vorlage in die neue Tabelle übernommen,
            // bevor die Spalten entfernt werden. Die Id wird von der Identity-Spalte vergeben,
            // damit die oben eingefügten Beispielvorlagen nicht überschrieben werden.
            migrationBuilder.Sql("""
                INSERT INTO [MailTemplates] ([RepairOrderStatusId], [IsEnabled], [SubjectTemplate], [BodyTemplate], [Order], [CreatedAt])
                SELECT [TriggerStatusId], [AutoSendEnabled], [SubjectTemplate], [BodyTemplate], 0, SYSUTCDATETIME()
                FROM [MailSettings]
                WHERE [TriggerStatusId] IS NOT NULL
                  AND ([SubjectTemplate] <> '' OR [BodyTemplate] <> '');
                """);

            // Erst nach der Datenübernahme werden die nicht mehr benötigten Spalten entfernt.
            migrationBuilder.DropColumn(
                name: "AutoSendEnabled",
                table: "MailSettings");

            migrationBuilder.DropColumn(
                name: "BodyTemplate",
                table: "MailSettings");

            migrationBuilder.DropColumn(
                name: "SubjectTemplate",
                table: "MailSettings");

            migrationBuilder.DropColumn(
                name: "TriggerStatusId",
                table: "MailSettings");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrderMailLogs_MailTemplateId",
                table: "RepairOrderMailLogs",
                column: "MailTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_RepairOrderStatusId",
                table: "MailTemplates",
                column: "RepairOrderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairOrderMailLogs_MailTemplates_MailTemplateId",
                table: "RepairOrderMailLogs",
                column: "MailTemplateId",
                principalTable: "MailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairOrderMailLogs_MailTemplates_MailTemplateId",
                table: "RepairOrderMailLogs");

            migrationBuilder.DropTable(
                name: "MailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_RepairOrderMailLogs_MailTemplateId",
                table: "RepairOrderMailLogs");

            migrationBuilder.DropColumn(
                name: "MailTemplateId",
                table: "RepairOrderMailLogs");

            migrationBuilder.AddColumn<bool>(
                name: "AutoSendEnabled",
                table: "MailSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BodyTemplate",
                table: "MailSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectTemplate",
                table: "MailSettings",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TriggerStatusId",
                table: "MailSettings",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MailSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AutoSendEnabled", "BodyTemplate", "SubjectTemplate", "TriggerStatusId" },
                values: new object[] { false, "<p>Guten Tag {{CustomerName}},</p>\r\n<p>der Status Ihres Reparaturauftrags <strong>{{OrderNumber}}</strong>\r\n(Seriennummer {{SerialNumber}}) hat sich geändert:</p>\r\n<p><strong>{{Status}}</strong> – Stand: {{CurrentStateAt}}</p>\r\n<p>Bei Fragen antworten Sie einfach auf diese E-Mail.</p>\r\n<p>Mit freundlichen Grüßen<br />Ihr CamCare-Team</p>", "Ihr Reparaturauftrag {{OrderNumber}} – {{Status}}", null });

            migrationBuilder.CreateIndex(
                name: "IX_MailSettings_TriggerStatusId",
                table: "MailSettings",
                column: "TriggerStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailSettings_RepairOrderStatuses_TriggerStatusId",
                table: "MailSettings",
                column: "TriggerStatusId",
                principalTable: "RepairOrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
