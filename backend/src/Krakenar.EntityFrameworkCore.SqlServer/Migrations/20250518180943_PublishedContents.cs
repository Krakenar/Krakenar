using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class PublishedContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Content",
                table: "ContentLocales",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Content",
                table: "ContentLocales",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "Content",
                table: "ContentLocales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PublishedBy",
                schema: "Content",
                table: "ContentLocales",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedOn",
                schema: "Content",
                table: "ContentLocales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PublishedVersion",
                schema: "Content",
                table: "ContentLocales",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PublishedContents",
                schema: "Content",
                columns: table => new
                {
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "bit", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    PublishedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedContents", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Content",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedBy",
                schema: "Content",
                table: "ContentLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedOn",
                schema: "Content",
                table: "ContentLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_IsPublished",
                schema: "Content",
                table: "ContentLocales",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedBy",
                schema: "Content",
                table: "ContentLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedOn",
                schema: "Content",
                table: "ContentLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedVersion",
                schema: "Content",
                table: "ContentLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedBy",
                schema: "Content",
                table: "ContentLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedOn",
                schema: "Content",
                table: "ContentLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_Version",
                schema: "Content",
                table: "ContentLocales",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentId",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeId",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeName",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeUid",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentUid",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_DisplayName",
                schema: "Content",
                table: "PublishedContents",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageCode",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageId",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageIsDefault",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageUid",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedBy",
                schema: "Content",
                table: "PublishedContents",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedOn",
                schema: "Content",
                table: "PublishedContents",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueName",
                schema: "Content",
                table: "PublishedContents",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueNameNormalized",
                schema: "Content",
                table: "PublishedContents",
                column: "UniqueNameNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_Version",
                schema: "Content",
                table: "PublishedContents",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedContents",
                schema: "Content");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_CreatedBy",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_CreatedOn",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_IsPublished",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_PublishedBy",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_PublishedOn",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_PublishedVersion",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_UpdatedBy",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_UpdatedOn",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_Version",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropColumn(
                name: "PublishedBy",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropColumn(
                name: "PublishedOn",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.DropColumn(
                name: "PublishedVersion",
                schema: "Content",
                table: "ContentLocales");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "Content",
                table: "ContentLocales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Content",
                table: "ContentLocales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
