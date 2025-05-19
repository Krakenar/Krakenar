using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldIndex",
                schema: "Content",
                columns: table => new
                {
                    FieldIndexId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "bit", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Boolean = table.Column<bool>(type: "bit", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Number = table.Column<double>(type: "float", nullable: true),
                    RelatedContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RichText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Select = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    String = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldIndex", x => x.FieldIndexId);
                    table.ForeignKey(
                        name: "FK_FieldIndex_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Content",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldIndex_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalSchema: "Content",
                        principalTable: "FieldDefinitions",
                        principalColumn: "FieldDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldIndex_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Content",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniqueIndex",
                schema: "Content",
                columns: table => new
                {
                    UniqueIndexId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "bit", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ValueNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(278)", maxLength: 278, nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueIndex", x => x.UniqueIndexId);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Content",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalSchema: "Content",
                        principalTable: "FieldDefinitions",
                        principalColumn: "FieldDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Content",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Boolean",
                schema: "Content",
                table: "FieldIndex",
                column: "Boolean");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentId",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleId",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleId_FieldDefinitionId_Status",
                schema: "Content",
                table: "FieldIndex",
                columns: new[] { "ContentLocaleId", "FieldDefinitionId", "Status" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleName",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeId",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeName",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeUid",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentUid",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_DateTime",
                schema: "Content",
                table: "FieldIndex",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionId",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionName",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionUid",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeId",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeName",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeUid",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageCode",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageId",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageIsDefault",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageUid",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Number",
                schema: "Content",
                table: "FieldIndex",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Status",
                schema: "Content",
                table: "FieldIndex",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_String",
                schema: "Content",
                table: "FieldIndex",
                column: "String");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Version",
                schema: "Content",
                table: "FieldIndex",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentId",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleId",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleName",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeId",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeName",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId_LanguageId_Status_ValueNormalized",
                schema: "Content",
                table: "UniqueIndex",
                columns: new[] { "FieldDefinitionId", "LanguageId", "Status", "ValueNormalized" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionName",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeId",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeName",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Key",
                schema: "Content",
                table: "UniqueIndex",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageCode",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageId",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageIsDefault",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Status",
                schema: "Content",
                table: "UniqueIndex",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Value",
                schema: "Content",
                table: "UniqueIndex",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ValueNormalized",
                schema: "Content",
                table: "UniqueIndex",
                column: "ValueNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Version",
                schema: "Content",
                table: "UniqueIndex",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldIndex",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "UniqueIndex",
                schema: "Content");
        }
    }
}
