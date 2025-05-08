using Krakenar.Contracts.Messages;
using Krakenar.Core;
using Krakenar.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipientEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Recipient;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class RecipientConfiguration : IEntityTypeConfiguration<RecipientEntity>
{
  public void Configure(EntityTypeBuilder<RecipientEntity> builder)
  {
    builder.ToTable(KrakenarDb.Recipients.Table.Table ?? string.Empty, KrakenarDb.Recipients.Table.Schema);
    builder.HasKey(x => x.RecipientId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Type);
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.PhoneE164Formatted);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.UserUid);
    builder.HasIndex(x => x.UserUniqueName);
    builder.HasIndex(x => x.UserFullName);

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<RecipientType>());
    builder.Property(x => x.EmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(Phone.CountryCodeMaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.PhoneExtension).HasMaxLength(Phone.ExtensionMaximumLength);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(UserConfiguration.PhoneE164FormattedMaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.UserUniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UserFullName).HasMaxLength(UserConfiguration.FullNameMaximumLength);
    builder.Property(x => x.UserPicture).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Message).WithMany(x => x.Recipients)
      .HasPrincipalKey(x => x.MessageId).HasForeignKey(x => x.MessageId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.User).WithMany(x => x.Recipients)
      .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
