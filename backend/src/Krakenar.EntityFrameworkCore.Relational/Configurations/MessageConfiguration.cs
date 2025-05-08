using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;
using Krakenar.Core;
using Krakenar.Core.Localization;
using Krakenar.Core.Templates;
using Krakenar.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MessageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Message;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class MessageConfiguration : AggregateConfiguration<MessageEntity>, IEntityTypeConfiguration<MessageEntity>
{
  public override void Configure(EntityTypeBuilder<MessageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Messages.Table.Table ?? string.Empty, KrakenarDb.Messages.Table.Schema);
    builder.HasKey(x => x.MessageId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.BodyType);
    builder.HasIndex(x => x.RecipientCount);
    builder.HasIndex(x => x.SenderUid);
    builder.HasIndex(x => x.SenderProvider);
    builder.HasIndex(x => x.TemplateUid);
    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => x.IsDemo);
    builder.HasIndex(x => x.Status);

    builder.Property(x => x.Subject).HasMaxLength(Subject.MaximumLength);
    builder.Property(x => x.BodyType).HasMaxLength(TemplateConfiguration.ContentTypeMaximumLength);
    builder.Property(x => x.SenderEmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.SenderPhoneCountryCode).HasMaxLength(Phone.CountryCodeMaximumLength);
    builder.Property(x => x.SenderPhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.SenderPhoneExtension).HasMaxLength(Phone.ExtensionMaximumLength);
    builder.Property(x => x.SenderPhoneE164Formatted).HasMaxLength(UserConfiguration.PhoneE164FormattedMaximumLength);
    builder.Property(x => x.SenderDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.SenderProvider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());
    builder.Property(x => x.TemplateUniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.TemplateDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Locale).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<MessageStatus>());

    builder.HasOne(x => x.Realm).WithMany(x => x.Messages)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Sender).WithMany(x => x.Messages)
      .HasPrincipalKey(x => x.SenderId).HasForeignKey(x => x.SenderId)
      .OnDelete(DeleteBehavior.SetNull);
    builder.HasOne(x => x.Template).WithMany(x => x.Messages)
      .HasPrincipalKey(x => x.TemplateId).HasForeignKey(x => x.TemplateId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
