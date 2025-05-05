using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateApiKeyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                schema: "Identity",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecretHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthenticatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomAttributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.ApiKeyId);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyRoles",
                schema: "Identity",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyRoles", x => new { x.ApiKeyId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ApiKeyRoles_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalSchema: "Identity",
                        principalTable: "ApiKeys",
                        principalColumn: "ApiKeyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiKeyRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyRoles_RoleId",
                schema: "Identity",
                table: "ApiKeyRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_AuthenticatedOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "AuthenticatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedBy",
                schema: "Identity",
                table: "ApiKeys",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_ExpiresOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Name",
                schema: "Identity",
                table: "ApiKeys",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_RealmId_Id",
                schema: "Identity",
                table: "ApiKeys",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_RealmUid",
                schema: "Identity",
                table: "ApiKeys",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_StreamId",
                schema: "Identity",
                table: "ApiKeys",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UpdatedBy",
                schema: "Identity",
                table: "ApiKeys",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UpdatedOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Version",
                schema: "Identity",
                table: "ApiKeys",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ApiKeys",
                schema: "Identity");
        }
    }
}
