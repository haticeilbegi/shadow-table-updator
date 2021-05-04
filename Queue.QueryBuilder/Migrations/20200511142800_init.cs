using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Queue.QueryBuilder.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Queries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AuditType = table.Column<int>(nullable: false),
                    CommandText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Databases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ModuleId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Server = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Databases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Databases_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schemas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DatabaseId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schemas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schemas_Databases_DatabaseId",
                        column: x => x.DatabaseId,
                        principalTable: "Databases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SchemaId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OriginalTableId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tables_Tables_OriginalTableId",
                        column: x => x.OriginalTableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tables_Schemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "Schemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Columns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TableId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsNumeric = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Columns_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0d355012-e67f-47d2-8637-c3fdf80affe0"), "Ortak" },
                    { new Guid("4c1ebf36-c5e5-42f1-876d-f17b2e9b5f8d"), "NTE" }
                });

            migrationBuilder.InsertData(
                table: "Queries",
                columns: new[] { "Id", "AuditType", "CommandText" },
                values: new object[,]
                {
                    { new Guid("3963623e-3992-4ee5-950e-74738688d3b7"), 0, "INSERT INTO [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] ( {#COLUMNS#} ) VALUES ( {#VALUES#} )" },
                    { new Guid("10ae65a0-b02f-4025-9db0-58baaeed49ba"), 1, "UPDATE [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] SET {#COLUMNS#} WHERE {#CONDITION#}" },
                    { new Guid("37cdea2b-c762-4e4e-97e6-19f068aa4379"), 2, "UPDATE [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] SET {#COLUMN#} = {#VALUE#} WHERE {#CONDITION#}" },
                    { new Guid("0bc5e0e0-fbed-4c66-a2bd-8f13caca07f7"), 3, "DELETE FROM [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] WHERE {#CONDITION#}" }
                });

            migrationBuilder.InsertData(
                table: "Databases",
                columns: new[] { "Id", "ModuleId", "Name", "Password", "Server", "User" },
                values: new object[] { new Guid("9300b4a1-1686-4cdb-88d8-2d55dc52f8da"), new Guid("0d355012-e67f-47d2-8637-c3fdf80affe0"), "tenant_dev", "P5-4x/vR+", "nctestdb01.e-cozum.com", "tenant_dev" });

            migrationBuilder.InsertData(
                table: "Databases",
                columns: new[] { "Id", "ModuleId", "Name", "Password", "Server", "User" },
                values: new object[] { new Guid("2be20fd1-411a-4a48-bf0b-1404c5ee312d"), new Guid("4c1ebf36-c5e5-42f1-876d-f17b2e9b5f8d"), "nte_dev", "Hatice123", "(local)", "sa" });

            migrationBuilder.InsertData(
                table: "Schemas",
                columns: new[] { "Id", "DatabaseId", "Name" },
                values: new object[] { new Guid("51cad30e-a3f0-4dc4-80f2-d529d66152dc"), new Guid("9300b4a1-1686-4cdb-88d8-2d55dc52f8da"), "dbo" });

            migrationBuilder.InsertData(
                table: "Schemas",
                columns: new[] { "Id", "DatabaseId", "Name" },
                values: new object[] { new Guid("af227557-af3d-4763-9397-6f5d7f90e519"), new Guid("2be20fd1-411a-4a48-bf0b-1404c5ee312d"), "dbo" });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Name", "OriginalTableId", "SchemaId" },
                values: new object[] { new Guid("e7b28b74-afb7-4e71-b546-69208c3bc72c"), "Currencies", null, new Guid("51cad30e-a3f0-4dc4-80f2-d529d66152dc") });

            migrationBuilder.InsertData(
                table: "Columns",
                columns: new[] { "Id", "IsNumeric", "Name", "TableId" },
                values: new object[,]
                {
                    { new Guid("0b6c2448-6104-469d-841d-28b21e3fbcc9"), true, "Id", new Guid("e7b28b74-afb7-4e71-b546-69208c3bc72c") },
                    { new Guid("0871a44b-4502-46c5-9fc5-ec695a4e5d62"), false, "Code", new Guid("e7b28b74-afb7-4e71-b546-69208c3bc72c") },
                    { new Guid("2de28a51-da57-4a08-9173-e2abe5bc5f31"), false, "Name", new Guid("e7b28b74-afb7-4e71-b546-69208c3bc72c") },
                    { new Guid("9bdb0cf5-6c9e-4233-963b-766dd9423e33"), false, "Symbol", new Guid("e7b28b74-afb7-4e71-b546-69208c3bc72c") }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Name", "OriginalTableId", "SchemaId" },
                values: new object[] { new Guid("a96a54c8-ea74-4a0c-863b-f6a352987f68"), "Currencies", new Guid("e7b28b74-afb7-4e71-b546-69208c3bc72c"), new Guid("af227557-af3d-4763-9397-6f5d7f90e519") });

            migrationBuilder.InsertData(
                table: "Columns",
                columns: new[] { "Id", "IsNumeric", "Name", "TableId" },
                values: new object[,]
                {
                    { new Guid("e4ad2ffd-8496-49d6-b230-e99d79fdd046"), true, "Id", new Guid("a96a54c8-ea74-4a0c-863b-f6a352987f68") },
                    { new Guid("3601d63c-f3ad-4c63-9a25-1662c7c73709"), false, "Code", new Guid("a96a54c8-ea74-4a0c-863b-f6a352987f68") },
                    { new Guid("e26b3981-d685-4152-83e7-33498e68e392"), false, "Name", new Guid("a96a54c8-ea74-4a0c-863b-f6a352987f68") },
                    { new Guid("02a12d45-9fc1-46ab-98ef-1360b5955c65"), false, "Symbol", new Guid("a96a54c8-ea74-4a0c-863b-f6a352987f68") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Columns_TableId",
                table: "Columns",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Databases_ModuleId",
                table: "Databases",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schemas_DatabaseId",
                table: "Schemas",
                column: "DatabaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_OriginalTableId",
                table: "Tables",
                column: "OriginalTableId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_SchemaId",
                table: "Tables",
                column: "SchemaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Columns");

            migrationBuilder.DropTable(
                name: "Queries");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Schemas");

            migrationBuilder.DropTable(
                name: "Databases");

            migrationBuilder.DropTable(
                name: "Modules");
        }
    }
}
