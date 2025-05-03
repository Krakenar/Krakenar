using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateTokenBlacklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenBlacklist",
                schema: "Identity",
                columns: table => new
                {
                    BlacklistedTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenBlacklist", x => x.BlacklistedTokenId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklist_ExpiresOn",
                schema: "Identity",
                table: "TokenBlacklist",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklist_TokenId",
                schema: "Identity",
                table: "TokenBlacklist",
                column: "TokenId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenBlacklist",
                schema: "Identity");
        }
    }
}
