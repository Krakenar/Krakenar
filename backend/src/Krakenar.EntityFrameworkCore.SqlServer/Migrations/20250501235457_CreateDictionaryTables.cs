using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateDictionaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dictionaries",
                schema: "Localization",
                columns: table => new
                {
                    DictionaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    EntryCount = table.Column<int>(type: "int", nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.DictionaryId);
                    table.ForeignKey(
                        name: "FK_Dictionaries_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dictionaries_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryEntries",
                schema: "Localization",
                columns: table => new
                {
                    DictionaryEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DictionaryId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueShortened = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryEntries", x => x.DictionaryEntryId);
                    table.ForeignKey(
                        name: "FK_DictionaryEntries_Dictionaries_DictionaryId",
                        column: x => x.DictionaryId,
                        principalSchema: "Localization",
                        principalTable: "Dictionaries",
                        principalColumn: "DictionaryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedBy",
                schema: "Localization",
                table: "Dictionaries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedOn",
                schema: "Localization",
                table: "Dictionaries",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_EntryCount",
                schema: "Localization",
                table: "Dictionaries",
                column: "EntryCount");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_LanguageId",
                schema: "Localization",
                table: "Dictionaries",
                column: "LanguageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmId_Id",
                schema: "Localization",
                table: "Dictionaries",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmId_LanguageId",
                schema: "Localization",
                table: "Dictionaries",
                columns: new[] { "RealmId", "LanguageId" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmUid",
                schema: "Localization",
                table: "Dictionaries",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_StreamId",
                schema: "Localization",
                table: "Dictionaries",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedBy",
                schema: "Localization",
                table: "Dictionaries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedOn",
                schema: "Localization",
                table: "Dictionaries",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_Version",
                schema: "Localization",
                table: "Dictionaries",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_DictionaryId_Key",
                schema: "Localization",
                table: "DictionaryEntries",
                columns: new[] { "DictionaryId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_Key",
                schema: "Localization",
                table: "DictionaryEntries",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_ValueShortened",
                schema: "Localization",
                table: "DictionaryEntries",
                column: "ValueShortened");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryEntries",
                schema: "Localization");

            migrationBuilder.DropTable(
                name: "Dictionaries",
                schema: "Localization");
        }
    }
}
