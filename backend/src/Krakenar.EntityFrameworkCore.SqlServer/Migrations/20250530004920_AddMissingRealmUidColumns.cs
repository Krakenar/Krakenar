using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingRealmUidColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RealmUid",
                schema: "Identity",
                table: "UserIdentifiers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RealmUid",
                schema: "Identity",
                table: "Actors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_RealmUid",
                schema: "Identity",
                table: "UserIdentifiers",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_RealmUid",
                schema: "Identity",
                table: "Actors",
                column: "RealmUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserIdentifiers_RealmUid",
                schema: "Identity",
                table: "UserIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_Actors_RealmUid",
                schema: "Identity",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "RealmUid",
                schema: "Identity",
                table: "UserIdentifiers");

            migrationBuilder.DropColumn(
                name: "RealmUid",
                schema: "Identity",
                table: "Actors");
        }
    }
}
