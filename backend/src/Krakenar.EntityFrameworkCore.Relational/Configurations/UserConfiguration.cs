using Krakenar.Core;
using Krakenar.Core.Localization;
using Krakenar.Core.Users;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeZone = Krakenar.Core.Localization.TimeZone;
using UserEntity = Krakenar.EntityFrameworkCore.Relational.Entities.User;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class UserConfiguration : AggregateConfiguration<UserEntity>, IEntityTypeConfiguration<UserEntity>
{
  public const int AddressFormattedMaximumLength = Address.MaximumLength * 5 + 4; // NOTE(fpion): enough space to contain the five address components, each separated by one character.
  public const int FullNameMaximumLength = PersonName.MaximumLength * 3 + 2; // NOTE(fpion): enough space to contain the first, middle and last names, separator by a space ' '.
  public const int PhoneE164FormattedMaximumLength = Phone.CountryCodeMaximumLength + 1 + Phone.NumberMaximumLength + 7 + Phone.ExtensionMaximumLength; // NOTE(fpion): enough space to contain the following format '{CountryCode} {Number}, ext. {Extension}'.

  public override void Configure(EntityTypeBuilder<UserEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Users.Table.Table ?? string.Empty, KrakenarDb.Users.Table.Schema);
    builder.HasKey(x => x.UserId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.RealmId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.PasswordChangedBy);
    builder.HasIndex(x => x.PasswordChangedOn);
    builder.HasIndex(x => x.HasPassword);
    builder.HasIndex(x => x.DisabledBy);
    builder.HasIndex(x => x.DisabledOn);
    builder.HasIndex(x => x.IsDisabled);
    builder.HasIndex(x => x.AddressStreet);
    builder.HasIndex(x => x.AddressLocality);
    builder.HasIndex(x => x.AddressPostalCode);
    builder.HasIndex(x => x.AddressRegion);
    builder.HasIndex(x => x.AddressCountry);
    builder.HasIndex(x => x.AddressFormatted);
    builder.HasIndex(x => x.AddressVerifiedBy);
    builder.HasIndex(x => x.AddressVerifiedOn);
    builder.HasIndex(x => x.IsAddressVerified);
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => new { x.RealmId, x.EmailAddressNormalized });
    builder.HasIndex(x => x.EmailVerifiedBy);
    builder.HasIndex(x => x.EmailVerifiedOn);
    builder.HasIndex(x => x.IsEmailVerified);
    builder.HasIndex(x => x.PhoneCountryCode);
    builder.HasIndex(x => x.PhoneNumber);
    builder.HasIndex(x => x.PhoneExtension);
    builder.HasIndex(x => x.PhoneE164Formatted);
    builder.HasIndex(x => x.PhoneVerifiedBy);
    builder.HasIndex(x => x.PhoneVerifiedOn);
    builder.HasIndex(x => x.IsPhoneVerified);
    builder.HasIndex(x => x.IsConfirmed);
    builder.HasIndex(x => x.FirstName);
    builder.HasIndex(x => x.MiddleName);
    builder.HasIndex(x => x.LastName);
    builder.HasIndex(x => x.FullName);
    builder.HasIndex(x => x.Nickname);
    builder.HasIndex(x => x.Birthdate);
    builder.HasIndex(x => x.Gender);
    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => x.TimeZone);
    builder.HasIndex(x => x.AuthenticatedOn);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.PasswordHash).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordChangedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.DisabledBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.AddressStreet).HasMaxLength(Address.MaximumLength);
    builder.Property(x => x.AddressLocality).HasMaxLength(Address.MaximumLength);
    builder.Property(x => x.AddressPostalCode).HasMaxLength(Address.MaximumLength);
    builder.Property(x => x.AddressRegion).HasMaxLength(Address.MaximumLength);
    builder.Property(x => x.AddressCountry).HasMaxLength(Address.MaximumLength);
    builder.Property(x => x.AddressFormatted).HasMaxLength(AddressFormattedMaximumLength);
    builder.Property(x => x.AddressVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.EmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.EmailAddressNormalized).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.EmailVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(Phone.CountryCodeMaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.PhoneExtension).HasMaxLength(Phone.ExtensionMaximumLength);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(PhoneE164FormattedMaximumLength);
    builder.Property(x => x.PhoneVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.FirstName).HasMaxLength(PersonName.MaximumLength);
    builder.Property(x => x.MiddleName).HasMaxLength(PersonName.MaximumLength);
    builder.Property(x => x.LastName).HasMaxLength(PersonName.MaximumLength);
    builder.Property(x => x.FullName).HasMaxLength(FullNameMaximumLength);
    builder.Property(x => x.Nickname).HasMaxLength(PersonName.MaximumLength);
    builder.Property(x => x.Gender).HasMaxLength(Gender.MaximumLength);
    builder.Property(x => x.Locale).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.TimeZone).HasMaxLength(TimeZone.MaximumLength);
    builder.Property(x => x.Picture).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Profile).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Website).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.Users)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasMany(x => x.Roles).WithMany(x => x.Users).UsingEntity<UserRole>(joinBuilder =>
    {
      joinBuilder.ToTable(KrakenarDb.UserRoles.Table.Table ?? string.Empty, KrakenarDb.UserRoles.Table.Schema);
      joinBuilder.HasKey(x => new { x.UserId, x.RoleId });
    });
  }
}
