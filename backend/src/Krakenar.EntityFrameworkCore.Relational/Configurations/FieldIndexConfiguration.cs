using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Localization;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Identifier = Krakenar.Core.Identifier;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class FieldIndexConfiguration : IEntityTypeConfiguration<FieldIndex>
{
  public void Configure(EntityTypeBuilder<FieldIndex> builder)
  {
    builder.ToTable(KrakenarDb.FieldIndex.Table.Table ?? string.Empty, KrakenarDb.FieldIndex.Table.Schema);
    builder.HasKey(x => x.FieldIndexId);

    builder.HasIndex(x => x.ContentTypeId);
    builder.HasIndex(x => x.ContentTypeUid);
    builder.HasIndex(x => x.ContentTypeName);
    builder.HasIndex(x => x.LanguageId);
    builder.HasIndex(x => x.LanguageUid);
    builder.HasIndex(x => x.LanguageCode);
    builder.HasIndex(x => x.LanguageIsDefault);
    builder.HasIndex(x => x.FieldTypeId);
    builder.HasIndex(x => x.FieldTypeUid);
    builder.HasIndex(x => x.FieldTypeName);
    builder.HasIndex(x => x.FieldDefinitionId);
    builder.HasIndex(x => x.FieldDefinitionUid);
    builder.HasIndex(x => x.FieldDefinitionName);
    builder.HasIndex(x => x.ContentId);
    builder.HasIndex(x => x.ContentUid);
    builder.HasIndex(x => x.ContentLocaleId);
    builder.HasIndex(x => x.ContentLocaleName);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.Boolean);
    builder.HasIndex(x => x.DateTime);
    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.String);
    builder.HasIndex(x => new { x.ContentLocaleId, x.FieldDefinitionId, x.Status }).IsUnique();

    builder.Property(x => x.ContentTypeName).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.LanguageCode).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.FieldTypeName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.FieldDefinitionName).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(10).HasConversion(new EnumToStringConverter<ContentStatus>()); // NOTE(fpion): the longest status name, Published, is only 9 characters.
    builder.Property(x => x.ContentLocaleName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.String).HasMaxLength(FieldIndex.MaximumLength);

    builder.HasOne(x => x.ContentType).WithMany(x => x.FieldIndex)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Language).WithMany(x => x.FieldIndex)
      .HasPrincipalKey(x => x.LanguageId).HasForeignKey(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldType).WithMany(x => x.FieldIndex)
      .HasPrincipalKey(x => x.FieldTypeId).HasForeignKey(x => x.FieldTypeId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldDefinition).WithMany(x => x.FieldIndex)
      .HasPrincipalKey(x => x.FieldDefinitionId).HasForeignKey(x => x.FieldDefinitionId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Content).WithMany(x => x.FieldIndex)
      .HasPrincipalKey(x => x.ContentId).HasForeignKey(x => x.ContentId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.ContentLocale).WithMany(x => x.FieldIndex)
      .HasPrincipalKey(x => x.ContentLocaleId).HasForeignKey(x => x.ContentLocaleId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
