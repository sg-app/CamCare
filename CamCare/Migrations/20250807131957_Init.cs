using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CamCare.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CameraTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Defectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogisticProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairOrderStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    FontColor = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOrderClose = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairOrderStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Artikelnummer = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    AddressType = table.Column<int>(type: "INTEGER", nullable: false),
                    Street = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    HouseNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    CameraTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.SerialNumber);
                    table.ForeignKey(
                        name: "FK_Cameras_CameraTypes_CameraTypeId",
                        column: x => x.CameraTypeId,
                        principalTable: "CameraTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cameras_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepairOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    CameraSerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    RepairOrderStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShippingMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    LogisticProviderId = table.Column<int>(type: "INTEGER", nullable: true),
                    ArrivedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepairOrders_Cameras_CameraSerialNumber",
                        column: x => x.CameraSerialNumber,
                        principalTable: "Cameras",
                        principalColumn: "SerialNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepairOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepairOrders_LogisticProviders_LogisticProviderId",
                        column: x => x.LogisticProviderId,
                        principalTable: "LogisticProviders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RepairOrders_RepairOrderStatuses_RepairOrderStatusId",
                        column: x => x.RepairOrderStatusId,
                        principalTable: "RepairOrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefectiveRepairOrder",
                columns: table => new
                {
                    DefectivesId = table.Column<int>(type: "INTEGER", nullable: false),
                    RepairOrdersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectiveRepairOrder", x => new { x.DefectivesId, x.RepairOrdersId });
                    table.ForeignKey(
                        name: "FK_DefectiveRepairOrder_Defectives_DefectivesId",
                        column: x => x.DefectivesId,
                        principalTable: "Defectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefectiveRepairOrder_RepairOrders_RepairOrdersId",
                        column: x => x.RepairOrdersId,
                        principalTable: "RepairOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepairOrderRepairPosition",
                columns: table => new
                {
                    RepairOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    RepairPositionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RepairPostionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairOrderRepairPosition", x => new { x.RepairOrderId, x.RepairPositionId });
                    table.ForeignKey(
                        name: "FK_RepairOrderRepairPosition_RepairOrders_RepairOrderId",
                        column: x => x.RepairOrderId,
                        principalTable: "RepairOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepairOrderRepairPosition_RepairPositions_RepairPositionId",
                        column: x => x.RepairPositionId,
                        principalTable: "RepairPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CameraTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Mini 3000" },
                    { 2, "Mini 3110" },
                    { 3, "4540" },
                    { 4, "5030" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CompanyName", "CreatedAt", "Email", "FirstName", "LastName", "PhoneNumber", "UpdatedAt" },
                values: new object[] { "1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "max@mustermann.de", "Max", "Mustermann", "089190815", null });

            migrationBuilder.InsertData(
                table: "Defectives",
                columns: new[] { "Id", "CreatedAt", "Description", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Display defekt", null },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Objektiv defekt", null },
                    { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Akku defekt", null },
                    { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gehäuse defekt", null }
                });

            migrationBuilder.InsertData(
                table: "LogisticProviders",
                columns: new[] { "Id", "CreatedAt", "IsActive", "IsDefault", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "DHL", null },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Dachser", null },
                    { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "DPD", null }
                });

            migrationBuilder.InsertData(
                table: "RepairOrderStatuses",
                columns: new[] { "Id", "BackgroundColor", "CreatedAt", "Description", "FontColor", "IsActive", "IsDefault", "IsOrderClose", "Name", "Order", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "rgb(206, 206, 206)", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reparatur wurde von Kunden angemeldet.", "rgb(0, 0, 0)", true, true, false, "In Anlieferung", 1, null },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reparatur ist im Lager eingetroffen.", null, true, false, false, "Eingetroffen", 2, null },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reparatur wird von Mitarbeiter begutachtet.", null, true, false, false, "Begutachtung", 3, null },
                    { 4, "rgb(76, 170, 232)", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Begutachtung wurde vom Mitarbeiter abgeschlosen.", "rgb(0, 0, 0)", true, false, false, "Begutachtung abgeschlossen", 4, null },
                    { 5, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Angebot wurde erstellt.", null, true, false, false, "Angebot erstellt", 5, null },
                    { 6, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kamera befindet sich in der Reparatur.", null, true, false, false, "Reparatur", 6, null },
                    { 7, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reparatur kann nicht fortgesetzt werden da Ersatzteile bestellt wurden.", null, true, false, false, "Warte auf Ersatzteile", 7, null },
                    { 8, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kamera ist fertig repariert.", null, true, false, false, "Reparatur fertig", 8, null },
                    { 9, "rgb(125, 218, 88)", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kamera wurde versendet.", "rgb(0, 0, 0)", true, false, false, "Versendet", 9, null }
                });

            migrationBuilder.InsertData(
                table: "RepairPositions",
                columns: new[] { "Id", "Artikelnummer", "CreatedAt", "Description", "SortOrder", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Display tauschen", 0, null },
                    { 2, "01532", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Objektiv tauschen", 0, null },
                    { 3, "0153215", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Akku tauschen", 0, null },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gehäuse tauschen", 0, null }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AddressType", "City", "Country", "CreatedAt", "CustomerId", "HouseNumber", "PostalCode", "State", "Street", "UpdatedAt" },
                values: new object[] { 1, 1, "München", "Deutschland", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1", "", "80331", null, "Musterstraße 1", null });

            migrationBuilder.InsertData(
                table: "Cameras",
                columns: new[] { "SerialNumber", "CameraTypeId", "CreatedAt", "CustomerId", "UpdatedAt" },
                values: new object[] { "1234567890", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1", null });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_CameraTypeId",
                table: "Cameras",
                column: "CameraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_CustomerId",
                table: "Cameras",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectiveRepairOrder_RepairOrdersId",
                table: "DefectiveRepairOrder",
                column: "RepairOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrderRepairPosition_RepairPositionId",
                table: "RepairOrderRepairPosition",
                column: "RepairPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrders_CameraSerialNumber",
                table: "RepairOrders",
                column: "CameraSerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrders_CustomerId",
                table: "RepairOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrders_LogisticProviderId",
                table: "RepairOrders",
                column: "LogisticProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairOrders_RepairOrderStatusId",
                table: "RepairOrders",
                column: "RepairOrderStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "DefectiveRepairOrder");

            migrationBuilder.DropTable(
                name: "RepairOrderRepairPosition");

            migrationBuilder.DropTable(
                name: "Defectives");

            migrationBuilder.DropTable(
                name: "RepairOrders");

            migrationBuilder.DropTable(
                name: "RepairPositions");

            migrationBuilder.DropTable(
                name: "Cameras");

            migrationBuilder.DropTable(
                name: "LogisticProviders");

            migrationBuilder.DropTable(
                name: "RepairOrderStatuses");

            migrationBuilder.DropTable(
                name: "CameraTypes");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
