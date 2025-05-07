using Krakenar.Contracts.Senders;
using Krakenar.Core;
using Krakenar.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class SenderConfiguration : AggregateConfiguration<Entities.Sender>, IEntityTypeConfiguration<Entities.Sender>
{
  public const int PhoneE164FormattedMaximumLength = Phone.CountryCodeMaximumLength + 1 + Phone.NumberMaximumLength + 7 + Phone.ExtensionMaximumLength; // NOTE(fpion): enough space to contain the following format '{CountryCode} {Number}, ext. {Extension}'.

  public override void Configure(EntityTypeBuilder<Entities.Sender> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Senders.Table.Table ?? string.Empty, KrakenarDb.Senders.Table.Schema);
    builder.HasKey(x => x.SenderId);

    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Kind);
    builder.HasIndex(x => new { x.RealmId, x.Kind, x.IsDefault });
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.PhoneE164Formatted);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Provider);

    builder.Property(x => x.Kind).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderKind>());
    builder.Property(x => x.EmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(Phone.CountryCodeMaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(PhoneE164FormattedMaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Provider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());

    builder.HasOne(x => x.Realm).WithMany(x => x.Senders)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
