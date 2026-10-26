using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class AddMailSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmtpHost = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SmtpPort = table.Column<int>(type: "int", nullable: false),
                    Security = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false),
                    FromAddress = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    FromName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AutoSendEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TriggerStatusId = table.Column<int>(type: "int", nullable: true),
                    SubjectTemplate = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailSettings_RepairOrderStatuses_TriggerStatusId",
                        column: x => x.TriggerStatusId,
                        principalTable: "RepairOrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RepairOrderMailLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepairOrderId = table.Column<int>(type: "int", nullable: false),
                    RepairOrderStatusId = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Recipients = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairOrderMailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepairOrderMailLogs_RepairOrders_RepairOrderId",
                        column: x => x.RepairOrderId,
                        principalTable: "RepairOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MailSettings",
                columns: new[] { "Id", "ArchivedAt", "AutoSendEnabled", "BodyTemplate", "CreatedAt", "FromAddress", "FromName", "IsEnabled", "Password", "Security", "SmtpHost", "SmtpPort", "SubjectTemplate", "TimeoutSeconds", "TriggerStatusId", "UpdatedAt", "UserName" },
                values: new object[] { 1, null, false, "<p>Guten Tag {{CustomerName}},</p>\r\n<p>der Status Ihres Reparaturauftrags <strong>{{OrderNumber}}</strong>\r\n(Seriennummer {{SerialNumber}}) hat sich geändert:</p>\r\n<p><strong>{{Status}}</strong> – Stand: {{CurrentStateAt}}</p>\r\n<p>Bei Fragen antworten Sie einfach auf diese E-Mail.</p>\r\n<p>Mit freundlichen Grüßen<br />Ihr CamCare-Team</p>", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", null, false, null, 1, "", 587, "Ihr Reparaturauftrag {{OrderNumber}} – {{Status}}", 30, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_MailSettings_TriggerStatusId",
                table: "MailSettings",
                column: "TriggerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrderMailLogs_RepairOrderId",
                table: "RepairOrderMailLogs",
                column: "RepairOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailSettings");

            migrationBuilder.DropTable(
                name: "RepairOrderMailLogs");
        }
    }
}
