using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "Krakenar");

            migrationBuilder.CreateTable(
                name: "Configuration",
                schema: "Krakenar",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationId);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                schema: "Krakenar",
                columns: table => new
                {
                    RealmId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueSlug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueSlugNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Secret = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AllowedUniqueNameCharacters = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RequiredPasswordLength = table.Column<int>(type: "int", nullable: false),
                    RequiredPasswordUniqueChars = table.Column<int>(type: "int", nullable: false),
                    PasswordsRequireNonAlphanumeric = table.Column<bool>(type: "bit", nullable: false),
                    PasswordsRequireLowercase = table.Column<bool>(type: "bit", nullable: false),
                    PasswordsRequireUppercase = table.Column<bool>(type: "bit", nullable: false),
                    PasswordsRequireDigit = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHashingStrategy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RequireUniqueEmail = table.Column<bool>(type: "bit", nullable: false),
                    RequireConfirmedAccount = table.Column<bool>(type: "bit", nullable: false),
                    CustomAttributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.RealmId);
                });

            migrationBuilder.CreateTable(
                name: "Actors",
                schema: "Identity",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                    table.ForeignKey(
                        name: "FK_Actors_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "Identity",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    LCID = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CodeNormalized = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                    table.ForeignKey(
                        name: "FK_Languages_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomAttributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Roles_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PasswordChangedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PasswordChangedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasPassword = table.Column<bool>(type: "bit", nullable: false),
                    DisabledBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisabledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    AddressStreet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressLocality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressPostalCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressRegion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressCountry = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressFormatted = table.Column<string>(type: "nvarchar(1279)", maxLength: 1279, nullable: true),
                    AddressVerifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressVerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAddressVerified = table.Column<bool>(type: "bit", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmailAddressNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmailVerifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmailVerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhoneE164Formatted = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    PhoneVerifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneVerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPhoneVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(767)", maxLength: 767, nullable: true),
                    Nickname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Locale = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Picture = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Profile = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AuthenticatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomAttributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Identity",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SecretHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsPersistent = table.Column<bool>(type: "bit", nullable: false),
                    SignedOutBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SignedOutOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CustomAttributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentifiers",
                schema: "Identity",
                columns: table => new
                {
                    UserIdentifierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RealmId = table.Column<int>(type: "int", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentifiers", x => x.UserIdentifierId);
                    table.ForeignKey(
                        name: "FK_UserIdentifiers_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserIdentifiers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actors_DisplayName",
                schema: "Identity",
                table: "Actors",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_EmailAddress",
                schema: "Identity",
                table: "Actors",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_IsDeleted",
                schema: "Identity",
                table: "Actors",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_Key",
                schema: "Identity",
                table: "Actors",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actors_RealmId_Type_Id",
                schema: "Identity",
                table: "Actors",
                columns: new[] { "RealmId", "Type", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_CreatedBy",
                schema: "Krakenar",
                table: "Configuration",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_CreatedOn",
                schema: "Krakenar",
                table: "Configuration",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_Key",
                schema: "Krakenar",
                table: "Configuration",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_UpdatedBy",
                schema: "Krakenar",
                table: "Configuration",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_UpdatedOn",
                schema: "Krakenar",
                table: "Configuration",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_Value",
                schema: "Krakenar",
                table: "Configuration",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_Version",
                schema: "Krakenar",
                table: "Configuration",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                schema: "Identity",
                table: "Languages",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedBy",
                schema: "Identity",
                table: "Languages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedOn",
                schema: "Identity",
                table: "Languages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_DisplayName",
                schema: "Identity",
                table: "Languages",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_EnglishName",
                schema: "Identity",
                table: "Languages",
                column: "EnglishName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                schema: "Identity",
                table: "Languages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_NativeName",
                schema: "Identity",
                table: "Languages",
                column: "NativeName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmId_CodeNormalized",
                schema: "Identity",
                table: "Languages",
                columns: new[] { "RealmId", "CodeNormalized" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmId_Id",
                schema: "Identity",
                table: "Languages",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmId_LCID",
                schema: "Identity",
                table: "Languages",
                columns: new[] { "RealmId", "LCID" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmUid",
                schema: "Identity",
                table: "Languages",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_StreamId",
                schema: "Identity",
                table: "Languages",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedBy",
                schema: "Identity",
                table: "Languages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedOn",
                schema: "Identity",
                table: "Languages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Version",
                schema: "Identity",
                table: "Languages",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_CreatedBy",
                schema: "Krakenar",
                table: "Realms",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_CreatedOn",
                schema: "Krakenar",
                table: "Realms",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_DisplayName",
                schema: "Krakenar",
                table: "Realms",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_Id",
                schema: "Krakenar",
                table: "Realms",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_StreamId",
                schema: "Krakenar",
                table: "Realms",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UniqueSlug",
                schema: "Krakenar",
                table: "Realms",
                column: "UniqueSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UniqueSlugNormalized",
                schema: "Krakenar",
                table: "Realms",
                column: "UniqueSlugNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UpdatedBy",
                schema: "Krakenar",
                table: "Realms",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UpdatedOn",
                schema: "Krakenar",
                table: "Realms",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_Version",
                schema: "Krakenar",
                table: "Realms",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedBy",
                schema: "Identity",
                table: "Roles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedOn",
                schema: "Identity",
                table: "Roles",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DisplayName",
                schema: "Identity",
                table: "Roles",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RealmId_Id",
                schema: "Identity",
                table: "Roles",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RealmId_UniqueNameNormalized",
                schema: "Identity",
                table: "Roles",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RealmUid",
                schema: "Identity",
                table: "Roles",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_StreamId",
                schema: "Identity",
                table: "Roles",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UniqueName",
                schema: "Identity",
                table: "Roles",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UpdatedBy",
                schema: "Identity",
                table: "Roles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UpdatedOn",
                schema: "Identity",
                table: "Roles",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Version",
                schema: "Identity",
                table: "Roles",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedBy",
                schema: "Identity",
                table: "Sessions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedOn",
                schema: "Identity",
                table: "Sessions",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Id",
                schema: "Identity",
                table: "Sessions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IsActive",
                schema: "Identity",
                table: "Sessions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IsPersistent",
                schema: "Identity",
                table: "Sessions",
                column: "IsPersistent");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RealmId_Id",
                schema: "Identity",
                table: "Sessions",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RealmUid",
                schema: "Identity",
                table: "Sessions",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SignedOutBy",
                schema: "Identity",
                table: "Sessions",
                column: "SignedOutBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SignedOutOn",
                schema: "Identity",
                table: "Sessions",
                column: "SignedOutOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_StreamId",
                schema: "Identity",
                table: "Sessions",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedBy",
                schema: "Identity",
                table: "Sessions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedOn",
                schema: "Identity",
                table: "Sessions",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                schema: "Identity",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Version",
                schema: "Identity",
                table: "Sessions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_Key",
                schema: "Identity",
                table: "UserIdentifiers",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_RealmId_Key_Value",
                schema: "Identity",
                table: "UserIdentifiers",
                columns: new[] { "RealmId", "Key", "Value" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_UserId_Key",
                schema: "Identity",
                table: "UserIdentifiers",
                columns: new[] { "UserId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_Value",
                schema: "Identity",
                table: "UserIdentifiers",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressCountry",
                schema: "Identity",
                table: "Users",
                column: "AddressCountry");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressFormatted",
                schema: "Identity",
                table: "Users",
                column: "AddressFormatted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressLocality",
                schema: "Identity",
                table: "Users",
                column: "AddressLocality");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressPostalCode",
                schema: "Identity",
                table: "Users",
                column: "AddressPostalCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressRegion",
                schema: "Identity",
                table: "Users",
                column: "AddressRegion");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressStreet",
                schema: "Identity",
                table: "Users",
                column: "AddressStreet");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressVerifiedBy",
                schema: "Identity",
                table: "Users",
                column: "AddressVerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressVerifiedOn",
                schema: "Identity",
                table: "Users",
                column: "AddressVerifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthenticatedOn",
                schema: "Identity",
                table: "Users",
                column: "AuthenticatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Birthdate",
                schema: "Identity",
                table: "Users",
                column: "Birthdate");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                schema: "Identity",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedOn",
                schema: "Identity",
                table: "Users",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DisabledBy",
                schema: "Identity",
                table: "Users",
                column: "DisabledBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DisabledOn",
                schema: "Identity",
                table: "Users",
                column: "DisabledOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                schema: "Identity",
                table: "Users",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailVerifiedBy",
                schema: "Identity",
                table: "Users",
                column: "EmailVerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailVerifiedOn",
                schema: "Identity",
                table: "Users",
                column: "EmailVerifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                schema: "Identity",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                schema: "Identity",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Gender",
                schema: "Identity",
                table: "Users",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Users_HasPassword",
                schema: "Identity",
                table: "Users",
                column: "HasPassword");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsAddressVerified",
                schema: "Identity",
                table: "Users",
                column: "IsAddressVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsConfirmed",
                schema: "Identity",
                table: "Users",
                column: "IsConfirmed");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDisabled",
                schema: "Identity",
                table: "Users",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsEmailVerified",
                schema: "Identity",
                table: "Users",
                column: "IsEmailVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsPhoneVerified",
                schema: "Identity",
                table: "Users",
                column: "IsPhoneVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastName",
                schema: "Identity",
                table: "Users",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Locale",
                schema: "Identity",
                table: "Users",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MiddleName",
                schema: "Identity",
                table: "Users",
                column: "MiddleName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Nickname",
                schema: "Identity",
                table: "Users",
                column: "Nickname");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordChangedBy",
                schema: "Identity",
                table: "Users",
                column: "PasswordChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordChangedOn",
                schema: "Identity",
                table: "Users",
                column: "PasswordChangedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneCountryCode",
                schema: "Identity",
                table: "Users",
                column: "PhoneCountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneE164Formatted",
                schema: "Identity",
                table: "Users",
                column: "PhoneE164Formatted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneExtension",
                schema: "Identity",
                table: "Users",
                column: "PhoneExtension");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "Identity",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneVerifiedBy",
                schema: "Identity",
                table: "Users",
                column: "PhoneVerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneVerifiedOn",
                schema: "Identity",
                table: "Users",
                column: "PhoneVerifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmId_EmailAddressNormalized",
                schema: "Identity",
                table: "Users",
                columns: new[] { "RealmId", "EmailAddressNormalized" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmId_Id",
                schema: "Identity",
                table: "Users",
                columns: new[] { "RealmId", "Id" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmId_UniqueNameNormalized",
                schema: "Identity",
                table: "Users",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true,
                filter: "[RealmId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmUid",
                schema: "Identity",
                table: "Users",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StreamId",
                schema: "Identity",
                table: "Users",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TimeZone",
                schema: "Identity",
                table: "Users",
                column: "TimeZone");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UniqueName",
                schema: "Identity",
                table: "Users",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedBy",
                schema: "Identity",
                table: "Users",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedOn",
                schema: "Identity",
                table: "Users",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Version",
                schema: "Identity",
                table: "Users",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actors",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Configuration",
                schema: "Krakenar");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserIdentifiers",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Realms",
                schema: "Krakenar");
        }
    }
}
