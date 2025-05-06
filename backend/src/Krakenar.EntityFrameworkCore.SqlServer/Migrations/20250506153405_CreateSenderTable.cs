using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateSenderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Senders",
                schema: "Messaging",
                columns: table => new
                {
                    SenderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_Senders", x => x.SenderId);
                    table.ForeignKey(
                        name: "FK_Senders_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Senders_CreatedBy",
                schema: "Messaging",
                table: "Senders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_CreatedOn",
                schema: "Messaging",
                table: "Senders",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_DisplayName",
                schema: "Messaging",
                table: "Senders",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_EmailAddress",
                schema: "Messaging",
                table: "Senders",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Kind",
                schema: "Messaging",
                table: "Senders",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_PhoneNumber",
                schema: "Messaging",
                table: "Senders",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Provider",
                schema: "Messaging",
                table: "Senders",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmId_Id",
                schema: "Messaging",
                table: "Senders",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmId_Kind_IsDefault",
                schema: "Messaging",
                table: "Senders",
                columns: new[] { "RealmId", "Kind", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmUid",
                schema: "Messaging",
                table: "Senders",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_StreamId",
                schema: "Messaging",
                table: "Senders",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedBy",
                schema: "Messaging",
                table: "Senders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedOn",
                schema: "Messaging",
                table: "Senders",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Version",
                schema: "Messaging",
                table: "Senders",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Senders",
                schema: "Messaging");
        }
    }
}
