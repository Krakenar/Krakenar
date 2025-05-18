using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class ContentLocaleConfiguration : IEntityTypeConfiguration<ContentLocale>
{
  public void Configure(EntityTypeBuilder<ContentLocale> builder)
  {
    builder.ToTable(KrakenarDb.ContentLocales.Table.Table ?? string.Empty, KrakenarDb.ContentLocales.Table.Schema);
    builder.HasKey(x => x.ContentLocaleId);

    builder.HasIndex(x => x.ContentTypeUid);
    builder.HasIndex(x => x.ContentUid);
    builder.HasIndex(x => new { x.ContentId, x.LanguageId }).IsUnique();
    builder.HasIndex(x => x.LanguageUid);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.ContentTypeId, x.LanguageId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);
    builder.HasIndex(x => x.IsPublished);
    builder.HasIndex(x => x.PublishedVersion);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.PublishedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.ContentType).WithMany(x => x.ContentLocales)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Content).WithMany(x => x.Locales)
      .HasPrincipalKey(x => x.ContentId).HasForeignKey(x => x.ContentId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Language).WithMany(x => x.ContentLocales)
      .HasPrincipalKey(x => x.LanguageId).HasForeignKey(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
