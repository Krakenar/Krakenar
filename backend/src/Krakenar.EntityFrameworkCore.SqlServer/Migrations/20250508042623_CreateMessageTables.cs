using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateMessageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "Messaging",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BodyType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientCount = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    SenderUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderIsDefault = table.Column<bool>(type: "bit", nullable: false),
                    SenderEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SenderPhoneCountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    SenderPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SenderPhoneExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SenderPhoneE164Formatted = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    SenderDisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SenderProvider = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: true),
                    TemplateUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateUniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateDisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IgnoreUserLocale = table.Column<bool>(type: "bit", nullable: false),
                    Locale = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDemo = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Senders_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "Messaging",
                        principalTable: "Senders",
                        principalColumn: "SenderId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Messages_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "Messaging",
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                schema: "Messaging",
                columns: table => new
                {
                    RecipientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhoneE164Formatted = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserUniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserFullName = table.Column<string>(type: "nvarchar(767)", maxLength: 767, nullable: true),
                    UserPicture = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.RecipientId);
                    table.ForeignKey(
                        name: "FK_Recipients_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Messaging",
                        principalTable: "Messages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recipients_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_BodyType",
                schema: "Messaging",
                table: "Messages",
                column: "BodyType");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedBy",
                schema: "Messaging",
                table: "Messages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedOn",
                schema: "Messaging",
                table: "Messages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsDemo",
                schema: "Messaging",
                table: "Messages",
                column: "IsDemo");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Locale",
                schema: "Messaging",
                table: "Messages",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmId_Id",
                schema: "Messaging",
                table: "Messages",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmUid",
                schema: "Messaging",
                table: "Messages",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientCount",
                schema: "Messaging",
                table: "Messages",
                column: "RecipientCount");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                schema: "Messaging",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderProvider",
                schema: "Messaging",
                table: "Messages",
                column: "SenderProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderUid",
                schema: "Messaging",
                table: "Messages",
                column: "SenderUid");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Status",
                schema: "Messaging",
                table: "Messages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_StreamId",
                schema: "Messaging",
                table: "Messages",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Subject",
                schema: "Messaging",
                table: "Messages",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateId",
                schema: "Messaging",
                table: "Messages",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateUid",
                schema: "Messaging",
                table: "Messages",
                column: "TemplateUid");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedBy",
                schema: "Messaging",
                table: "Messages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedOn",
                schema: "Messaging",
                table: "Messages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Version",
                schema: "Messaging",
                table: "Messages",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_DisplayName",
                schema: "Messaging",
                table: "Recipients",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_EmailAddress",
                schema: "Messaging",
                table: "Recipients",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_Id",
                schema: "Messaging",
                table: "Recipients",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_MessageId",
                schema: "Messaging",
                table: "Recipients",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_PhoneE164Formatted",
                schema: "Messaging",
                table: "Recipients",
                column: "PhoneE164Formatted");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_Type",
                schema: "Messaging",
                table: "Recipients",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_UserFullName",
                schema: "Messaging",
                table: "Recipients",
                column: "UserFullName");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_UserId",
                schema: "Messaging",
                table: "Recipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_UserUid",
                schema: "Messaging",
                table: "Recipients",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_UserUniqueName",
                schema: "Messaging",
                table: "Recipients",
                column: "UserUniqueName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipients",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "Messaging");
        }
    }
}
