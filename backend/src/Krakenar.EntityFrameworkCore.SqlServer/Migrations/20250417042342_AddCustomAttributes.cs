using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomAttributes",
                schema: "Identity",
                columns: table => new
                {
                    CustomAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueShortened = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAttributes", x => x.CustomAttributeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomAttributes_Entity_Key",
                schema: "Identity",
                table: "CustomAttributes",
                columns: new[] { "Entity", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomAttributes_Key",
                schema: "Identity",
                table: "CustomAttributes",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_CustomAttributes_ValueShortened",
                schema: "Identity",
                table: "CustomAttributes",
                column: "ValueShortened");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomAttributes",
                schema: "Identity");
        }
    }
}
