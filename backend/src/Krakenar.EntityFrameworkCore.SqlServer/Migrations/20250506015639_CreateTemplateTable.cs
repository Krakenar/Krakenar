using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateTemplateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Templates",
                schema: "Identity",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ContentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_Templates_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Templates_ContentType",
                schema: "Identity",
                table: "Templates",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedBy",
                schema: "Identity",
                table: "Templates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedOn",
                schema: "Identity",
                table: "Templates",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_DisplayName",
                schema: "Identity",
                table: "Templates",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmId_Id",
                schema: "Identity",
                table: "Templates",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmId_UniqueNameNormalized",
                schema: "Identity",
                table: "Templates",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmUid",
                schema: "Identity",
                table: "Templates",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_StreamId",
                schema: "Identity",
                table: "Templates",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Subject",
                schema: "Identity",
                table: "Templates",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UniqueName",
                schema: "Identity",
                table: "Templates",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedBy",
                schema: "Identity",
                table: "Templates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedOn",
                schema: "Identity",
                table: "Templates",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Version",
                schema: "Identity",
                table: "Templates",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Templates",
                schema: "Identity");
        }
    }
}
