using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateFieldTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Content");

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                schema: "Content",
                columns: table => new
                {
                    FieldTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.FieldTypeId);
                    table.ForeignKey(
                        name: "FK_FieldTypes_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedBy",
                schema: "Content",
                table: "FieldTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedOn",
                schema: "Content",
                table: "FieldTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DataType",
                schema: "Content",
                table: "FieldTypes",
                column: "DataType");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DisplayName",
                schema: "Content",
                table: "FieldTypes",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RealmId_Id",
                schema: "Content",
                table: "FieldTypes",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RealmId_UniqueNameNormalized",
                schema: "Content",
                table: "FieldTypes",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RealmUid",
                schema: "Content",
                table: "FieldTypes",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_StreamId",
                schema: "Content",
                table: "FieldTypes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UniqueName",
                schema: "Content",
                table: "FieldTypes",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedBy",
                schema: "Content",
                table: "FieldTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedOn",
                schema: "Content",
                table: "FieldTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_Version",
                schema: "Content",
                table: "FieldTypes",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldTypes",
                schema: "Content");
        }
    }
}
