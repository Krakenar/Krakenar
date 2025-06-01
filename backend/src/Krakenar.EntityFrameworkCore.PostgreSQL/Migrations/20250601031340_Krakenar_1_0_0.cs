using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Krakenar.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class Krakenar_1_0_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "Krakenar");

            migrationBuilder.EnsureSchema(
                name: "Content");

            migrationBuilder.EnsureSchema(
                name: "Localization");

            migrationBuilder.EnsureSchema(
                name: "Logging");

            migrationBuilder.EnsureSchema(
                name: "Messaging");

            migrationBuilder.CreateTable(
                name: "Configuration",
                schema: "Krakenar",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationId);
                });

            migrationBuilder.CreateTable(
                name: "CustomAttributes",
                schema: "Identity",
                columns: table => new
                {
                    CustomAttributeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Entity = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ValueShortened = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAttributes", x => x.CustomAttributeId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "Logging",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Method = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Destination = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Source = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    OperationType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OperationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActivityData = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    RealmId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ApiKeyId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActorId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                schema: "Krakenar",
                columns: table => new
                {
                    RealmId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueSlug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueSlugNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Secret = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SecretChangedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SecretChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    AllowedUniqueNameCharacters = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RequiredPasswordLength = table.Column<int>(type: "integer", nullable: false),
                    RequiredPasswordUniqueChars = table.Column<int>(type: "integer", nullable: false),
                    PasswordsRequireNonAlphanumeric = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireLowercase = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireUppercase = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireDigit = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHashingStrategy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RequireUniqueEmail = table.Column<bool>(type: "boolean", nullable: false),
                    RequireConfirmedAccount = table.Column<bool>(type: "boolean", nullable: false),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.RealmId);
                });

            migrationBuilder.CreateTable(
                name: "TokenBlacklist",
                schema: "Identity",
                columns: table => new
                {
                    BlacklistedTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenBlacklist", x => x.BlacklistedTokenId);
                });

            migrationBuilder.CreateTable(
                name: "LogEvents",
                schema: "Logging",
                columns: table => new
                {
                    LogEventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogId = table.Column<long>(type: "bigint", nullable: false),
                    LogUid = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEvents", x => x.LogEventId);
                    table.ForeignKey(
                        name: "FK_LogEvents_Logs_LogId",
                        column: x => x.LogId,
                        principalSchema: "Logging",
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogExceptions",
                schema: "Logging",
                columns: table => new
                {
                    LogExceptionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogId = table.Column<long>(type: "bigint", nullable: false),
                    LogUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    HResult = table.Column<int>(type: "integer", nullable: false),
                    HelpLink = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    TargetSite = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogExceptions", x => x.LogExceptionId);
                    table.ForeignKey(
                        name: "FK_LogExceptions_Logs_LogId",
                        column: x => x.LogId,
                        principalSchema: "Logging",
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Actors",
                schema: "Identity",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PictureUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
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
                name: "ApiKeys",
                schema: "Identity",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SecretHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AuthenticatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.ApiKeyId);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentTypes",
                schema: "Content",
                columns: table => new
                {
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsInvariant = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FieldCount = table.Column<int>(type: "integer", nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTypes", x => x.ContentTypeId);
                    table.ForeignKey(
                        name: "FK_ContentTypes_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "Localization",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    LCID = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CodeNormalized = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EnglishName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NativeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "Senders",
                schema: "Messaging",
                columns: table => new
                {
                    SenderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhoneE164Formatted = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Settings = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.SenderId);
                    table.ForeignKey(
                        name: "FK_Senders_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                schema: "Messaging",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ContentText = table.Column<string>(type: "text", nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_Templates_Realms_RealmId",
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
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordChangedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HasPassword = table.Column<bool>(type: "boolean", nullable: false),
                    DisabledBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DisabledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    AddressStreet = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLocality = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressPostalCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressRegion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressCountry = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressFormatted = table.Column<string>(type: "character varying(1279)", maxLength: 1279, nullable: true),
                    AddressVerifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAddressVerified = table.Column<bool>(type: "boolean", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailAddressNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailVerifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhoneExtension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PhoneE164Formatted = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PhoneVerifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPhoneVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "character varying(767)", maxLength: 767, nullable: true),
                    Nickname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Picture = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Profile = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Website = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    AuthenticatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "Contents",
                schema: "Content",
                columns: table => new
                {
                    ContentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.ContentId);
                    table.ForeignKey(
                        name: "FK_Contents_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contents_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                schema: "Content",
                columns: table => new
                {
                    FieldTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Settings = table.Column<string>(type: "text", nullable: true),
                    RelatedContentTypeId = table.Column<int>(type: "integer", nullable: true),
                    RelatedContentTypeUid = table.Column<Guid>(type: "uuid", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.FieldTypeId);
                    table.ForeignKey(
                        name: "FK_FieldTypes_ContentTypes_RelatedContentTypeId",
                        column: x => x.RelatedContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldTypes_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dictionaries",
                schema: "Localization",
                columns: table => new
                {
                    DictionaryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    LanguageUid = table.Column<Guid>(type: "uuid", nullable: false),
                    EntryCount = table.Column<int>(type: "integer", nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.DictionaryId);
                    table.ForeignKey(
                        name: "FK_Dictionaries_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dictionaries_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyRoles",
                schema: "Identity",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyRoles", x => new { x.ApiKeyId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ApiKeyRoles_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalSchema: "Identity",
                        principalTable: "ApiKeys",
                        principalColumn: "ApiKeyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiKeyRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "Messaging",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BodyType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    BodyText = table.Column<string>(type: "text", nullable: false),
                    RecipientCount = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: true),
                    SenderUid = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    SenderEmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SenderPhoneCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    SenderPhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SenderPhoneExtension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    SenderPhoneE164Formatted = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    SenderDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SenderProvider = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TemplateId = table.Column<int>(type: "integer", nullable: true),
                    TemplateUid = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateUniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TemplateDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IgnoreUserLocale = table.Column<bool>(type: "boolean", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Variables = table.Column<string>(type: "text", nullable: true),
                    IsDemo = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Results = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "OneTimePasswords",
                schema: "Identity",
                columns: table => new
                {
                    OneTimePasswordId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    UserUid = table.Column<Guid>(type: "uuid", nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaximumAttempts = table.Column<int>(type: "integer", nullable: true),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    ValidationSucceededOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneTimePasswords", x => x.OneTimePasswordId);
                    table.ForeignKey(
                        name: "FK_OneTimePasswords_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneTimePasswords_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Identity",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserUid = table.Column<Guid>(type: "uuid", nullable: false),
                    SecretHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsPersistent = table.Column<bool>(type: "boolean", nullable: false),
                    SignedOutBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SignedOutOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    UserIdentifierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserUid = table.Column<Guid>(type: "uuid", nullable: false),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
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
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ContentLocales",
                schema: "Content",
                columns: table => new
                {
                    ContentLocaleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentId = table.Column<int>(type: "integer", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uuid", nullable: true),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FieldValues = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedVersion = table.Column<long>(type: "bigint", nullable: true),
                    PublishedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentLocales", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_ContentLocales_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentLocales_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentLocales_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldDefinitions",
                schema: "Content",
                columns: table => new
                {
                    FieldDefinitionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    FieldTypeId = table.Column<int>(type: "integer", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsInvariant = table.Column<bool>(type: "boolean", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsIndexed = table.Column<bool>(type: "boolean", nullable: false),
                    IsUnique = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Placeholder = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "DictionaryEntries",
                schema: "Localization",
                columns: table => new
                {
                    DictionaryEntryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DictionaryId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ValueShortened = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryEntries", x => x.DictionaryEntryId);
                    table.ForeignKey(
                        name: "FK_DictionaryEntries_Dictionaries_DictionaryId",
                        column: x => x.DictionaryId,
                        principalSchema: "Localization",
                        principalTable: "Dictionaries",
                        principalColumn: "DictionaryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                schema: "Messaging",
                columns: table => new
                {
                    RecipientId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhoneExtension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PhoneE164Formatted = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    UserUid = table.Column<Guid>(type: "uuid", nullable: true),
                    UserUniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserFullName = table.Column<string>(type: "character varying(767)", maxLength: 767, nullable: true),
                    UserPicture = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "PublishedContents",
                schema: "Content",
                columns: table => new
                {
                    ContentLocaleId = table.Column<int>(type: "integer", nullable: false),
                    ContentId = table.Column<int>(type: "integer", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uuid", nullable: true),
                    LanguageCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FieldValues = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    PublishedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedContents", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Content",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldIndex",
                schema: "Content",
                columns: table => new
                {
                    FieldIndexId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uuid", nullable: true),
                    LanguageCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    FieldTypeId = table.Column<int>(type: "integer", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentId = table.Column<int>(type: "integer", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "integer", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Boolean = table.Column<bool>(type: "boolean", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Number = table.Column<double>(type: "double precision", nullable: true),
                    RelatedContent = table.Column<string>(type: "text", nullable: true),
                    RichText = table.Column<string>(type: "text", nullable: true),
                    Select = table.Column<string>(type: "text", nullable: true),
                    String = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldIndex", x => x.FieldIndexId);
                    table.ForeignKey(
                        name: "FK_FieldIndex_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Content",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldIndex_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalSchema: "Content",
                        principalTable: "FieldDefinitions",
                        principalColumn: "FieldDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldIndex_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Content",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UniqueIndex",
                schema: "Content",
                columns: table => new
                {
                    UniqueIndexId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    RealmUid = table.Column<Guid>(type: "uuid", nullable: true),
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uuid", nullable: true),
                    LanguageCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    FieldTypeId = table.Column<int>(type: "integer", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ValueNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "character varying(278)", maxLength: 278, nullable: false),
                    ContentId = table.Column<int>(type: "integer", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "integer", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueIndex", x => x.UniqueIndexId);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Content",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Content",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Content",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalSchema: "Content",
                        principalTable: "FieldDefinitions",
                        principalColumn: "FieldDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Content",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Localization",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Realms_RealmId",
                        column: x => x.RealmId,
                        principalSchema: "Krakenar",
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actors_RealmUid",
                schema: "Identity",
                table: "Actors",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyRoles_RoleId",
                schema: "Identity",
                table: "ApiKeyRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_AuthenticatedOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "AuthenticatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedBy",
                schema: "Identity",
                table: "ApiKeys",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_ExpiresOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Name",
                schema: "Identity",
                table: "ApiKeys",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_RealmId_Id",
                schema: "Identity",
                table: "ApiKeys",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_RealmUid",
                schema: "Identity",
                table: "ApiKeys",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_StreamId",
                schema: "Identity",
                table: "ApiKeys",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UpdatedBy",
                schema: "Identity",
                table: "ApiKeys",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UpdatedOn",
                schema: "Identity",
                table: "ApiKeys",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Version",
                schema: "Identity",
                table: "ApiKeys",
                column: "Version");

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
                name: "IX_ContentLocales_ContentId_LanguageId",
                schema: "Content",
                table: "ContentLocales",
                columns: new[] { "ContentId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentTypeId_LanguageId_UniqueNameNormalized",
                schema: "Content",
                table: "ContentLocales",
                columns: new[] { "ContentTypeId", "LanguageId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentTypeUid",
                schema: "Content",
                table: "ContentLocales",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentUid",
                schema: "Content",
                table: "ContentLocales",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedBy",
                schema: "Content",
                table: "ContentLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedOn",
                schema: "Content",
                table: "ContentLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_DisplayName",
                schema: "Content",
                table: "ContentLocales",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_IsPublished",
                schema: "Content",
                table: "ContentLocales",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_LanguageId",
                schema: "Content",
                table: "ContentLocales",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_LanguageUid",
                schema: "Content",
                table: "ContentLocales",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedBy",
                schema: "Content",
                table: "ContentLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedOn",
                schema: "Content",
                table: "ContentLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedVersion",
                schema: "Content",
                table: "ContentLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UniqueName",
                schema: "Content",
                table: "ContentLocales",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedBy",
                schema: "Content",
                table: "ContentLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedOn",
                schema: "Content",
                table: "ContentLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_Version",
                schema: "Content",
                table: "ContentLocales",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ContentTypeId",
                schema: "Content",
                table: "Contents",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ContentTypeUid",
                schema: "Content",
                table: "Contents",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedBy",
                schema: "Content",
                table: "Contents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedOn",
                schema: "Content",
                table: "Contents",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_RealmId_Id",
                schema: "Content",
                table: "Contents",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_RealmUid",
                schema: "Content",
                table: "Contents",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_StreamId",
                schema: "Content",
                table: "Contents",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UpdatedBy",
                schema: "Content",
                table: "Contents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UpdatedOn",
                schema: "Content",
                table: "Contents",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_Version",
                schema: "Content",
                table: "Contents",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_CreatedBy",
                schema: "Content",
                table: "ContentTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_CreatedOn",
                schema: "Content",
                table: "ContentTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_DisplayName",
                schema: "Content",
                table: "ContentTypes",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_FieldCount",
                schema: "Content",
                table: "ContentTypes",
                column: "FieldCount");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_IsInvariant",
                schema: "Content",
                table: "ContentTypes",
                column: "IsInvariant");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_RealmId_Id",
                schema: "Content",
                table: "ContentTypes",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_RealmId_UniqueNameNormalized",
                schema: "Content",
                table: "ContentTypes",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_RealmUid",
                schema: "Content",
                table: "ContentTypes",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_StreamId",
                schema: "Content",
                table: "ContentTypes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UniqueName",
                schema: "Content",
                table: "ContentTypes",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UpdatedBy",
                schema: "Content",
                table: "ContentTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UpdatedOn",
                schema: "Content",
                table: "ContentTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_Version",
                schema: "Content",
                table: "ContentTypes",
                column: "Version");

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

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedBy",
                schema: "Localization",
                table: "Dictionaries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedOn",
                schema: "Localization",
                table: "Dictionaries",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_EntryCount",
                schema: "Localization",
                table: "Dictionaries",
                column: "EntryCount");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_LanguageId",
                schema: "Localization",
                table: "Dictionaries",
                column: "LanguageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_LanguageUid",
                schema: "Localization",
                table: "Dictionaries",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmId_Id",
                schema: "Localization",
                table: "Dictionaries",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmId_LanguageId",
                schema: "Localization",
                table: "Dictionaries",
                columns: new[] { "RealmId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmUid",
                schema: "Localization",
                table: "Dictionaries",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_StreamId",
                schema: "Localization",
                table: "Dictionaries",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedBy",
                schema: "Localization",
                table: "Dictionaries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedOn",
                schema: "Localization",
                table: "Dictionaries",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_Version",
                schema: "Localization",
                table: "Dictionaries",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_DictionaryId_Key",
                schema: "Localization",
                table: "DictionaryEntries",
                columns: new[] { "DictionaryId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_Key",
                schema: "Localization",
                table: "DictionaryEntries",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_ValueShortened",
                schema: "Localization",
                table: "DictionaryEntries",
                column: "ValueShortened");

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

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Boolean",
                schema: "Content",
                table: "FieldIndex",
                column: "Boolean");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentId",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleId",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleId_FieldDefinitionId_Status",
                schema: "Content",
                table: "FieldIndex",
                columns: new[] { "ContentLocaleId", "FieldDefinitionId", "Status" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleName",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeId",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeName",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeUid",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentUid",
                schema: "Content",
                table: "FieldIndex",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_DateTime",
                schema: "Content",
                table: "FieldIndex",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionId",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionName",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionUid",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeId",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeName",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeUid",
                schema: "Content",
                table: "FieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageCode",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageId",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageIsDefault",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageUid",
                schema: "Content",
                table: "FieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Number",
                schema: "Content",
                table: "FieldIndex",
                column: "Number");

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

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Status",
                schema: "Content",
                table: "FieldIndex",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_String",
                schema: "Content",
                table: "FieldIndex",
                column: "String");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Version",
                schema: "Content",
                table: "FieldIndex",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedBy",
                schema: "Content",
                table: "FieldTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedOn",
                schema: "Content",
                table: "FieldTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DataType",
                schema: "Content",
                table: "FieldTypes",
                column: "DataType");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DisplayName",
                schema: "Content",
                table: "FieldTypes",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RealmId_Id",
                schema: "Content",
                table: "FieldTypes",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RealmId_UniqueNameNormalized",
                schema: "Content",
                table: "FieldTypes",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_RealmUid",
                schema: "Content",
                table: "FieldTypes",
                column: "RealmUid");

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

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_StreamId",
                schema: "Content",
                table: "FieldTypes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UniqueName",
                schema: "Content",
                table: "FieldTypes",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedBy",
                schema: "Content",
                table: "FieldTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedOn",
                schema: "Content",
                table: "FieldTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_Version",
                schema: "Content",
                table: "FieldTypes",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                schema: "Localization",
                table: "Languages",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedBy",
                schema: "Localization",
                table: "Languages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedOn",
                schema: "Localization",
                table: "Languages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_DisplayName",
                schema: "Localization",
                table: "Languages",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_EnglishName",
                schema: "Localization",
                table: "Languages",
                column: "EnglishName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                schema: "Localization",
                table: "Languages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_NativeName",
                schema: "Localization",
                table: "Languages",
                column: "NativeName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmId_CodeNormalized",
                schema: "Localization",
                table: "Languages",
                columns: new[] { "RealmId", "CodeNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmId_Id",
                schema: "Localization",
                table: "Languages",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmId_LCID",
                schema: "Localization",
                table: "Languages",
                columns: new[] { "RealmId", "LCID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RealmUid",
                schema: "Localization",
                table: "Languages",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_StreamId",
                schema: "Localization",
                table: "Languages",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedBy",
                schema: "Localization",
                table: "Languages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedOn",
                schema: "Localization",
                table: "Languages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Version",
                schema: "Localization",
                table: "Languages",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_LogEvents_LogId_EventId",
                schema: "Logging",
                table: "LogEvents",
                columns: new[] { "LogId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEvents_LogUid",
                schema: "Logging",
                table: "LogEvents",
                column: "LogUid");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_Id",
                schema: "Logging",
                table: "LogExceptions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_LogId",
                schema: "Logging",
                table: "LogExceptions",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_LogUid",
                schema: "Logging",
                table: "LogExceptions",
                column: "LogUid");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_Type",
                schema: "Logging",
                table: "LogExceptions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActivityType",
                schema: "Logging",
                table: "Logs",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActorId",
                schema: "Logging",
                table: "Logs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ApiKeyId",
                schema: "Logging",
                table: "Logs",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CorrelationId",
                schema: "Logging",
                table: "Logs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Duration",
                schema: "Logging",
                table: "Logs",
                column: "Duration");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_EndedOn",
                schema: "Logging",
                table: "Logs",
                column: "EndedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_HasErrors",
                schema: "Logging",
                table: "Logs",
                column: "HasErrors");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Id",
                schema: "Logging",
                table: "Logs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IsCompleted",
                schema: "Logging",
                table: "Logs",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Level",
                schema: "Logging",
                table: "Logs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Method",
                schema: "Logging",
                table: "Logs",
                column: "Method");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationName",
                schema: "Logging",
                table: "Logs",
                column: "OperationName");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationType",
                schema: "Logging",
                table: "Logs",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RealmId",
                schema: "Logging",
                table: "Logs",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SessionId",
                schema: "Logging",
                table: "Logs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StartedOn",
                schema: "Logging",
                table: "Logs",
                column: "StartedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StatusCode",
                schema: "Logging",
                table: "Logs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                schema: "Logging",
                table: "Logs",
                column: "UserId");

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
                unique: true);

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
                name: "IX_OneTimePasswords_AttemptCount",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "AttemptCount");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_CreatedBy",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_CreatedOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_ExpiresOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_Id",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_MaximumAttempts",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "MaximumAttempts");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_RealmId_Id",
                schema: "Identity",
                table: "OneTimePasswords",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_RealmUid",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_StreamId",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UpdatedBy",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UpdatedOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UserId",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UserUid",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_ValidationSucceededOn",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "ValidationSucceededOn");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_Version",
                schema: "Identity",
                table: "OneTimePasswords",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentId",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeId",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeName",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeUid",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentUid",
                schema: "Content",
                table: "PublishedContents",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_DisplayName",
                schema: "Content",
                table: "PublishedContents",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageCode",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageId",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageIsDefault",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageUid",
                schema: "Content",
                table: "PublishedContents",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedBy",
                schema: "Content",
                table: "PublishedContents",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedOn",
                schema: "Content",
                table: "PublishedContents",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueName",
                schema: "Content",
                table: "PublishedContents",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueNameNormalized",
                schema: "Content",
                table: "PublishedContents",
                column: "UniqueNameNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_Version",
                schema: "Content",
                table: "PublishedContents",
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
                name: "IX_Realms_SecretChangedBy",
                schema: "Krakenar",
                table: "Realms",
                column: "SecretChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_SecretChangedOn",
                schema: "Krakenar",
                table: "Realms",
                column: "SecretChangedOn");

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RealmId_UniqueNameNormalized",
                schema: "Identity",
                table: "Roles",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true);

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
                name: "IX_Senders_CreatedBy",
                schema: "Messaging",
                table: "Senders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_CreatedOn",
                schema: "Messaging",
                table: "Senders",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_DisplayName",
                schema: "Messaging",
                table: "Senders",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_EmailAddress",
                schema: "Messaging",
                table: "Senders",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Kind",
                schema: "Messaging",
                table: "Senders",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_PhoneE164Formatted",
                schema: "Messaging",
                table: "Senders",
                column: "PhoneE164Formatted");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Provider",
                schema: "Messaging",
                table: "Senders",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmId_Id",
                schema: "Messaging",
                table: "Senders",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmId_Kind_IsDefault",
                schema: "Messaging",
                table: "Senders",
                columns: new[] { "RealmId", "Kind", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmUid",
                schema: "Messaging",
                table: "Senders",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_StreamId",
                schema: "Messaging",
                table: "Senders",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedBy",
                schema: "Messaging",
                table: "Senders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedOn",
                schema: "Messaging",
                table: "Senders",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Version",
                schema: "Messaging",
                table: "Senders",
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
                unique: true);

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
                name: "IX_Sessions_UserUid",
                schema: "Identity",
                table: "Sessions",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Version",
                schema: "Identity",
                table: "Sessions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_ContentType",
                schema: "Messaging",
                table: "Templates",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedBy",
                schema: "Messaging",
                table: "Templates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedOn",
                schema: "Messaging",
                table: "Templates",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_DisplayName",
                schema: "Messaging",
                table: "Templates",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmId_Id",
                schema: "Messaging",
                table: "Templates",
                columns: new[] { "RealmId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmId_UniqueNameNormalized",
                schema: "Messaging",
                table: "Templates",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmUid",
                schema: "Messaging",
                table: "Templates",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_StreamId",
                schema: "Messaging",
                table: "Templates",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Subject",
                schema: "Messaging",
                table: "Templates",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UniqueName",
                schema: "Messaging",
                table: "Templates",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedBy",
                schema: "Messaging",
                table: "Templates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedOn",
                schema: "Messaging",
                table: "Templates",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Version",
                schema: "Messaging",
                table: "Templates",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklist_ExpiresOn",
                schema: "Identity",
                table: "TokenBlacklist",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklist_TokenId",
                schema: "Identity",
                table: "TokenBlacklist",
                column: "TokenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentId",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleId",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleName",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeId",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeName",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId_LanguageId_Status_ValueNormal~",
                schema: "Content",
                table: "UniqueIndex",
                columns: new[] { "FieldDefinitionId", "LanguageId", "Status", "ValueNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionName",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeId",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeName",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Key",
                schema: "Content",
                table: "UniqueIndex",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageCode",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageId",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageIsDefault",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageUid",
                schema: "Content",
                table: "UniqueIndex",
                column: "LanguageUid");

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
                name: "IX_UniqueIndex_Status",
                schema: "Content",
                table: "UniqueIndex",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Value",
                schema: "Content",
                table: "UniqueIndex",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ValueNormalized",
                schema: "Content",
                table: "UniqueIndex",
                column: "ValueNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Version",
                schema: "Content",
                table: "UniqueIndex",
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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_RealmUid",
                schema: "Identity",
                table: "UserIdentifiers",
                column: "RealmUid");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_UserId_Key",
                schema: "Identity",
                table: "UserIdentifiers",
                columns: new[] { "UserId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_UserUid",
                schema: "Identity",
                table: "UserIdentifiers",
                column: "UserUid");

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmId_UniqueNameNormalized",
                schema: "Identity",
                table: "Users",
                columns: new[] { "RealmId", "UniqueNameNormalized" },
                unique: true);

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
                name: "ApiKeyRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Configuration",
                schema: "Krakenar");

            migrationBuilder.DropTable(
                name: "CustomAttributes",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "DictionaryEntries",
                schema: "Localization");

            migrationBuilder.DropTable(
                name: "FieldIndex",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "LogEvents",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "LogExceptions",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "OneTimePasswords",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "PublishedContents",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Recipients",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TokenBlacklist",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UniqueIndex",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "UserIdentifiers",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ApiKeys",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Dictionaries",
                schema: "Localization");

            migrationBuilder.DropTable(
                name: "Logs",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "ContentLocales",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "FieldDefinitions",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Senders",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "Templates",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "Contents",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "Localization");

            migrationBuilder.DropTable(
                name: "FieldTypes",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "ContentTypes",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Realms",
                schema: "Krakenar");
        }
    }
}
