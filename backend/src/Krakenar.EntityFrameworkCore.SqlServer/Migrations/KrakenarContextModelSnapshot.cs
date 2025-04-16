﻿// <auto-generated />
using Krakenar.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Krakenar.EntityFrameworkCore.SqlServer.Migrations
{
    [DbContext(typeof(KrakenarContext))]
    partial class KrakenarContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Actor", b =>
                {
                    b.Property<int>("ActorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ActorId"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<int?>("RealmId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ActorId");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("RealmId", "Type", "Id")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.ToTable("Actors", "Identity");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Configuration", b =>
                {
                    b.Property<int>("ConfigurationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ConfigurationId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("ConfigurationId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Value");

                    b.HasIndex("Version");

                    b.ToTable("Configuration", "Krakenar");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Language", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LanguageId"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("CodeNormalized")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EnglishName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<int>("LCID")
                        .HasColumnType("int");

                    b.Property<string>("NativeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("RealmId")
                        .HasColumnType("int");

                    b.Property<Guid?>("RealmUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("LanguageId");

                    b.HasIndex("Code");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EnglishName");

                    b.HasIndex("IsDefault");

                    b.HasIndex("NativeName");

                    b.HasIndex("RealmUid");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("RealmId", "CodeNormalized")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.HasIndex("RealmId", "Id")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.HasIndex("RealmId", "LCID")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.ToTable("Languages", "Localization");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", b =>
                {
                    b.Property<int>("RealmId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RealmId"));

                    b.Property<string>("AllowedUniqueNameCharacters")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PasswordHashingStrategy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("PasswordsRequireDigit")
                        .HasColumnType("bit");

                    b.Property<bool>("PasswordsRequireLowercase")
                        .HasColumnType("bit");

                    b.Property<bool>("PasswordsRequireNonAlphanumeric")
                        .HasColumnType("bit");

                    b.Property<bool>("PasswordsRequireUppercase")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireConfirmedAccount")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireUniqueEmail")
                        .HasColumnType("bit");

                    b.Property<int>("RequiredPasswordLength")
                        .HasColumnType("int");

                    b.Property<int>("RequiredPasswordUniqueChars")
                        .HasColumnType("int");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueSlug")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueSlugNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RealmId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueSlug");

                    b.HasIndex("UniqueSlugNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Realms", "Krakenar");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("RealmId")
                        .HasColumnType("int");

                    b.Property<Guid?>("RealmUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RoleId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("RealmUid");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueName");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("RealmId", "Id")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.HasIndex("RealmId", "UniqueNameNormalized")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.ToTable("Roles", "Identity");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Session", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SessionId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPersistent")
                        .HasColumnType("bit");

                    b.Property<int?>("RealmId")
                        .HasColumnType("int");

                    b.Property<Guid?>("RealmUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SecretHash")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("SignedOutBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("SignedOutOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SessionId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("IsPersistent");

                    b.HasIndex("RealmUid");

                    b.HasIndex("SignedOutBy");

                    b.HasIndex("SignedOutOn");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("UserId");

                    b.HasIndex("Version");

                    b.HasIndex("RealmId", "Id")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.ToTable("Sessions", "Identity");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("AddressCountry")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AddressFormatted")
                        .HasMaxLength(1279)
                        .HasColumnType("nvarchar(1279)");

                    b.Property<string>("AddressLocality")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AddressPostalCode")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AddressRegion")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AddressStreet")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AddressVerifiedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("AddressVerifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("AuthenticatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisabledBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("DisabledOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EmailAddressNormalized")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EmailVerifiedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("EmailVerifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FullName")
                        .HasMaxLength(767)
                        .HasColumnType("nvarchar(767)");

                    b.Property<string>("Gender")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("HasPassword")
                        .HasColumnType("bit");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAddressVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPhoneVerified")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Locale")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Nickname")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PasswordChangedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("PasswordChangedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PhoneCountryCode")
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.Property<string>("PhoneE164Formatted")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("PhoneExtension")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PhoneVerifiedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("PhoneVerifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Picture")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("Profile")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<int?>("RealmId")
                        .HasColumnType("int");

                    b.Property<Guid?>("RealmUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TimeZone")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<string>("Website")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.HasKey("UserId");

                    b.HasIndex("AddressCountry");

                    b.HasIndex("AddressFormatted");

                    b.HasIndex("AddressLocality");

                    b.HasIndex("AddressPostalCode");

                    b.HasIndex("AddressRegion");

                    b.HasIndex("AddressStreet");

                    b.HasIndex("AddressVerifiedBy");

                    b.HasIndex("AddressVerifiedOn");

                    b.HasIndex("AuthenticatedOn");

                    b.HasIndex("Birthdate");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisabledBy");

                    b.HasIndex("DisabledOn");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("EmailVerifiedBy");

                    b.HasIndex("EmailVerifiedOn");

                    b.HasIndex("FirstName");

                    b.HasIndex("FullName");

                    b.HasIndex("Gender");

                    b.HasIndex("HasPassword");

                    b.HasIndex("IsAddressVerified");

                    b.HasIndex("IsConfirmed");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("IsEmailVerified");

                    b.HasIndex("IsPhoneVerified");

                    b.HasIndex("LastName");

                    b.HasIndex("Locale");

                    b.HasIndex("MiddleName");

                    b.HasIndex("Nickname");

                    b.HasIndex("PasswordChangedBy");

                    b.HasIndex("PasswordChangedOn");

                    b.HasIndex("PhoneCountryCode");

                    b.HasIndex("PhoneE164Formatted");

                    b.HasIndex("PhoneExtension");

                    b.HasIndex("PhoneNumber");

                    b.HasIndex("PhoneVerifiedBy");

                    b.HasIndex("PhoneVerifiedOn");

                    b.HasIndex("RealmUid");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("TimeZone");

                    b.HasIndex("UniqueName");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("RealmId", "EmailAddressNormalized");

                    b.HasIndex("RealmId", "Id")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.HasIndex("RealmId", "UniqueNameNormalized")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.ToTable("Users", "Identity");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.UserIdentifier", b =>
                {
                    b.Property<int>("UserIdentifierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserIdentifierId"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("RealmId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("UserIdentifierId");

                    b.HasIndex("Key");

                    b.HasIndex("Value");

                    b.HasIndex("UserId", "Key")
                        .IsUnique();

                    b.HasIndex("RealmId", "Key", "Value")
                        .IsUnique()
                        .HasFilter("[RealmId] IS NOT NULL");

                    b.ToTable("UserIdentifiers", "Identity");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", "Identity");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Actor", b =>
                {
                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", "Realm")
                        .WithMany("Actors")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Language", b =>
                {
                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", "Realm")
                        .WithMany("Languages")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Role", b =>
                {
                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", "Realm")
                        .WithMany("Roles")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Session", b =>
                {
                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", "Realm")
                        .WithMany("Sessions")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Realm");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.User", b =>
                {
                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", "Realm")
                        .WithMany("Users")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.UserIdentifier", b =>
                {
                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", "Realm")
                        .WithMany("UserIdentifiers")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.User", "User")
                        .WithMany("Identifiers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realm");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.UserRole", b =>
                {
                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Krakenar.EntityFrameworkCore.Relational.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.Realm", b =>
                {
                    b.Navigation("Actors");

                    b.Navigation("Languages");

                    b.Navigation("Roles");

                    b.Navigation("Sessions");

                    b.Navigation("UserIdentifiers");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Krakenar.EntityFrameworkCore.Relational.Entities.User", b =>
                {
                    b.Navigation("Identifiers");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
