using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateOTPTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OneTimePasswords",
                schema: "Identity",
                columns: table => new
                {
                    OneTimePasswordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaximumAttempts = table.Column<int>(type: "int", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    HasValidationSucceeded = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_OneTimePasswords", x => x.OneTimePasswordId);
                    table.ForeignKey(
                        name: "FK_OneTimePasswords_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneTimePasswords_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_AttemptCount",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "AttemptCount");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_CreatedBy",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_CreatedOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_ExpiresOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_HasValidationSucceeded",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "HasValidationSucceeded");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_Id",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_MaximumAttempts",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "MaximumAttempts");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_RealmId_Id",
                schema: "Identity",
                table: "OneTimePasswords",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_RealmUid",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_StreamId",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UpdatedBy",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UpdatedOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UserId",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_Version",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OneTimePasswords",
                schema: "Identity");
        }
    }
}
