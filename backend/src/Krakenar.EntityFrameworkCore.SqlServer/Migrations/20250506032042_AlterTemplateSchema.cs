using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AlterTemplateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Messaging");

            migrationBuilder.RenameTable(
                name: "Templates",
                schema: "Identity",
                newName: "Templates",
                newSchema: "Messaging");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Templates",
                schema: "Messaging",
                newName: "Templates",
                newSchema: "Identity");
        }
    }
}
