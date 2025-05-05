using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ValidationSucceededOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OneTimePasswords_HasValidationSucceeded",
                schema: "Identity",
                table: "OneTimePasswords");

            migrationBuilder.DropColumn(
                name: "HasValidationSucceeded",
                schema: "Identity",
                table: "OneTimePasswords");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidationSucceededOn",
                schema: "Identity",
                table: "OneTimePasswords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_ValidationSucceededOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "ValidationSucceededOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OneTimePasswords_ValidationSucceededOn",
                schema: "Identity",
                table: "OneTimePasswords");

            migrationBuilder.DropColumn(
                name: "ValidationSucceededOn",
                schema: "Identity",
                table: "OneTimePasswords");

            migrationBuilder.AddColumn<bool>(
                name: "HasValidationSucceeded",
                schema: "Identity",
                table: "OneTimePasswords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_HasValidationSucceeded",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "HasValidationSucceeded");
        }
    }
}
