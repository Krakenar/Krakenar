using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatedContentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedContentTypeId",
                schema: "Content",
                table: "FieldTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RelatedContentTypeUid",
                schema: "Content",
                table: "FieldTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RelatedContentTypeId",
                schema: "Content",
                table: "FieldTypes",
                column: "RelatedContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RelatedContentTypeUid",
                schema: "Content",
                table: "FieldTypes",
                column: "RelatedContentTypeUid");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldTypes_ContentTypes_RelatedContentTypeId",
                schema: "Content",
                table: "FieldTypes",
                column: "RelatedContentTypeId",
                principalSchema: "Content",
                principalTable: "ContentTypes",
                principalColumn: "ContentTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldTypes_ContentTypes_RelatedContentTypeId",
                schema: "Content",
                table: "FieldTypes");

            migrationBuilder.DropIndex(
                name: "IX_FieldTypes_RelatedContentTypeId",
                schema: "Content",
                table: "FieldTypes");

            migrationBuilder.DropIndex(
                name: "IX_FieldTypes_RelatedContentTypeUid",
                schema: "Content",
                table: "FieldTypes");

            migrationBuilder.DropColumn(
                name: "RelatedContentTypeId",
                schema: "Content",
                table: "FieldTypes");

            migrationBuilder.DropColumn(
                name: "RelatedContentTypeUid",
                schema: "Content",
                table: "FieldTypes");
        }
    }
}
