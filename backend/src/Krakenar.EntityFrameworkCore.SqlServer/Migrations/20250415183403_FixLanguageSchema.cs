using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class FixLanguageSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Localization");

            migrationBuilder.RenameTable(
                name: "Languages",
                schema: "Identity",
                newName: "Languages",
                newSchema: "Localization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Languages",
                schema: "Localization",
                newName: "Languages",
                newSchema: "Identity");
        }
    }
}
