using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateFieldDefinitionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldCount",
                schema: "Content",
                table: "ContentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FieldDefinitions",
                schema: "Content",
                columns: table => new
                {
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInvariant = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsIndexed = table.Column<bool>(type: "bit", nullable: false),
                    IsUnique = table.Column<bool>(type: "bit", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Placeholder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDefinitions", x => x.FieldDefinitionId);
                    table.ForeignKey(
                        name: "FK_FieldDefinitions_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldDefinitions_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Content",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_FieldCount",
                schema: "Content",
                table: "ContentTypes",
                column: "FieldCount");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_Id",
                schema: "Content",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_Order",
                schema: "Content",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_UniqueNameNormalized",
                schema: "Content",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeUid",
                schema: "Content",
                table: "FieldDefinitions",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_CreatedBy",
                schema: "Content",
                table: "FieldDefinitions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_CreatedOn",
                schema: "Content",
                table: "FieldDefinitions",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_DisplayName",
                schema: "Content",
                table: "FieldDefinitions",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_FieldTypeId",
                schema: "Content",
                table: "FieldDefinitions",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_FieldTypeUid",
                schema: "Content",
                table: "FieldDefinitions",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsIndexed",
                schema: "Content",
                table: "FieldDefinitions",
                column: "IsIndexed");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsInvariant",
                schema: "Content",
                table: "FieldDefinitions",
                column: "IsInvariant");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsRequired",
                schema: "Content",
                table: "FieldDefinitions",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsUnique",
                schema: "Content",
                table: "FieldDefinitions",
                column: "IsUnique");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_Placeholder",
                schema: "Content",
                table: "FieldDefinitions",
                column: "Placeholder");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_UniqueName",
                schema: "Content",
                table: "FieldDefinitions",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_UpdatedBy",
                schema: "Content",
                table: "FieldDefinitions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_UpdatedOn",
                schema: "Content",
                table: "FieldDefinitions",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_Version",
                schema: "Content",
                table: "FieldDefinitions",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldDefinitions",
                schema: "Content");

            migrationBuilder.DropIndex(
                name: "IX_ContentTypes_FieldCount",
                schema: "Content",
                table: "ContentTypes");

            migrationBuilder.DropColumn(
                name: "FieldCount",
                schema: "Content",
                table: "ContentTypes");
        }
    }
}
