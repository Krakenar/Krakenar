using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUidColumnsAndIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserUid",
                schema: "Identity",
                table: "UserIdentifiers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserUid",
                schema: "Identity",
                table: "Sessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserUid",
                schema: "Identity",
                table: "OneTimePasswords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LanguageUid",
                schema: "Localization",
                table: "Dictionaries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_UserUid",
                schema: "Identity",
                table: "UserIdentifiers",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserUid",
                schema: "Identity",
                table: "Sessions",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UserUid",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_LanguageUid",
                schema: "Localization",
                table: "Dictionaries",
                column: "LanguageUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserIdentifiers_UserUid",
                schema: "Identity",
                table: "UserIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserUid",
                schema: "Identity",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePasswords_UserUid",
                schema: "Identity",
                table: "OneTimePasswords");

            migrationBuilder.DropIndex(
                name: "IX_Dictionaries_LanguageUid",
                schema: "Localization",
                table: "Dictionaries");

            migrationBuilder.DropColumn(
                name: "UserUid",
                schema: "Identity",
                table: "UserIdentifiers");

            migrationBuilder.DropColumn(
                name: "UserUid",
                schema: "Identity",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "UserUid",
                schema: "Identity",
                table: "OneTimePasswords");

            migrationBuilder.DropColumn(
                name: "LanguageUid",
                schema: "Localization",
                table: "Dictionaries");
        }
    }
}
