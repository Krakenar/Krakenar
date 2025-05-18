using Krakenar.Core;
using Krakenar.Core.Localization;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class PublishedContentConfiguration : IEntityTypeConfiguration<PublishedContent>
{
  public void Configure(EntityTypeBuilder<PublishedContent> builder)
  {
    builder.ToTable(KrakenarDb.PublishedContents.Table.Table ?? string.Empty, KrakenarDb.PublishedContents.Table.Schema);
    builder.HasKey(x => x.ContentLocaleId);

    builder.HasIndex(x => x.ContentUid);
    builder.HasIndex(x => x.ContentTypeUid);
    builder.HasIndex(x => x.ContentTypeName);
    builder.HasIndex(x => x.LanguageUid);
    builder.HasIndex(x => x.LanguageCode);
    builder.HasIndex(x => x.LanguageIsDefault);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.ContentTypeName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.LanguageCode).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.PublishedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.ContentLocale).WithOne(x => x.PublishedContent)
      .HasPrincipalKey<ContentLocale>(x => x.ContentLocaleId)
      .HasForeignKey<PublishedContent>(x => x.ContentLocaleId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Content).WithMany(x => x.PublishedContents)
      .HasPrincipalKey(x => x.ContentId).HasForeignKey(x => x.ContentId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.ContentType).WithMany(x => x.PublishedContents)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Language).WithMany(x => x.PublishedContents)
      .HasPrincipalKey(x => x.LanguageId).HasForeignKey(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
