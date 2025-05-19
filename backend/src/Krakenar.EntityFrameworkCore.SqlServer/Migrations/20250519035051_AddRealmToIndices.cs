using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddRealmToIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RealmId",
                schema: "Content",
                table: "UniqueIndex",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RealmUid",
                schema: "Content",
                table: "UniqueIndex",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealmId",
                schema: "Content",
                table: "FieldIndex",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RealmUid",
                schema: "Content",
                table: "FieldIndex",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_RealmId",
                schema: "Content",
                table: "UniqueIndex",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_RealmUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_RealmId",
                schema: "Content",
                table: "FieldIndex",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_RealmUid",
                schema: "Content",
                table: "FieldIndex",
                column: "RealmUid");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldIndex_Realms_RealmId",
                schema: "Content",
                table: "FieldIndex",
                column: "RealmId",
                principalSchema: "Krakenar",
                principalTable: "Realms",
                principalColumn: "RealmId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UniqueIndex_Realms_RealmId",
                schema: "Content",
                table: "UniqueIndex",
                column: "RealmId",
                principalSchema: "Krakenar",
                principalTable: "Realms",
                principalColumn: "RealmId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldIndex_Realms_RealmId",
                schema: "Content",
                table: "FieldIndex");

            migrationBuilder.DropForeignKey(
                name: "FK_UniqueIndex_Realms_RealmId",
                schema: "Content",
                table: "UniqueIndex");

            migrationBuilder.DropIndex(
                name: "IX_UniqueIndex_RealmId",
                schema: "Content",
                table: "UniqueIndex");

            migrationBuilder.DropIndex(
                name: "IX_UniqueIndex_RealmUid",
                schema: "Content",
                table: "UniqueIndex");

            migrationBuilder.DropIndex(
                name: "IX_FieldIndex_RealmId",
                schema: "Content",
                table: "FieldIndex");

            migrationBuilder.DropIndex(
                name: "IX_FieldIndex_RealmUid",
                schema: "Content",
                table: "FieldIndex");

            migrationBuilder.DropColumn(
                name: "RealmId",
                schema: "Content",
                table: "UniqueIndex");

            migrationBuilder.DropColumn(
                name: "RealmUid",
                schema: "Content",
                table: "UniqueIndex");

            migrationBuilder.DropColumn(
                name: "RealmId",
                schema: "Content",
                table: "FieldIndex");

            migrationBuilder.DropColumn(
                name: "RealmUid",
                schema: "Content",
                table: "FieldIndex");
        }
    }
}
