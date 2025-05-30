using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddRealmSecretColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecretChangedBy",
                schema: "Krakenar",
                table: "Realms",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SecretChangedOn",
                schema: "Krakenar",
                table: "Realms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Realms_SecretChangedBy",
                schema: "Krakenar",
                table: "Realms",
                column: "SecretChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_SecretChangedOn",
                schema: "Krakenar",
                table: "Realms",
                column: "SecretChangedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Realms_SecretChangedBy",
                schema: "Krakenar",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_Realms_SecretChangedOn",
                schema: "Krakenar",
                table: "Realms");

            migrationBuilder.DropColumn(
                name: "SecretChangedBy",
                schema: "Krakenar",
                table: "Realms");

            migrationBuilder.DropColumn(
                name: "SecretChangedOn",
                schema: "Krakenar",
                table: "Realms");
        }
    }
}
