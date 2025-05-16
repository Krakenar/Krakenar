using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Entities;
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
    builder.HasIndex(x => new { x.ContentTypeId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);

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
