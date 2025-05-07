using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddSenderPhoneColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Senders_PhoneNumber",
                schema: "Messaging",
                table: "Senders");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "Messaging",
                table: "Senders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneCountryCode",
                schema: "Messaging",
                table: "Senders",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneE164Formatted",
                schema: "Messaging",
                table: "Senders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_PhoneE164Formatted",
                schema: "Messaging",
                table: "Senders",
                column: "PhoneE164Formatted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Senders_PhoneE164Formatted",
                schema: "Messaging",
                table: "Senders");

            migrationBuilder.DropColumn(
                name: "PhoneCountryCode",
                schema: "Messaging",
                table: "Senders");

            migrationBuilder.DropColumn(
                name: "PhoneE164Formatted",
                schema: "Messaging",
                table: "Senders");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "Messaging",
                table: "Senders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_PhoneNumber",
                schema: "Messaging",
                table: "Senders",
                column: "PhoneNumber");
        }
    }
}
