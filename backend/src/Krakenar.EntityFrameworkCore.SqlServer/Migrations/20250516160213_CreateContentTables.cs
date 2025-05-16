using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateContentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contents",
                schema: "Content",
                columns: table => new
                {
                    ContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.ContentId);
                    table.ForeignKey(
                        name: "FK_Contents_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contents_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentLocales",
                schema: "Content",
                columns: table => new
                {
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentLocales", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_ContentLocales_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentLocales_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentLocales_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentId_LanguageId",
                schema: "Content",
                table: "ContentLocales",
                columns: new[] { "ContentId", "LanguageId" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentTypeId_LanguageId_UniqueNameNormalized",
                schema: "Content",
                table: "ContentLocales",
                columns: new[] { "ContentTypeId", "LanguageId", "UniqueNameNormalized" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentTypeUid",
                schema: "Content",
                table: "ContentLocales",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentUid",
                schema: "Content",
                table: "ContentLocales",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_DisplayName",
                schema: "Content",
                table: "ContentLocales",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_LanguageId",
                schema: "Content",
                table: "ContentLocales",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_LanguageUid",
                schema: "Content",
                table: "ContentLocales",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UniqueName",
                schema: "Content",
                table: "ContentLocales",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ContentTypeId",
                schema: "Content",
                table: "Contents",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ContentTypeUid",
                schema: "Content",
                table: "Contents",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedBy",
                schema: "Content",
                table: "Contents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedOn",
                schema: "Content",
                table: "Contents",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_RealmId_Id",
                schema: "Content",
                table: "Contents",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_RealmUid",
                schema: "Content",
                table: "Contents",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_StreamId",
                schema: "Content",
                table: "Contents",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UpdatedBy",
                schema: "Content",
                table: "Contents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UpdatedOn",
                schema: "Content",
                table: "Contents",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_Version",
                schema: "Content",
                table: "Contents",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentLocales",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Contents",
                schema: "Content");
        }
    }
}
