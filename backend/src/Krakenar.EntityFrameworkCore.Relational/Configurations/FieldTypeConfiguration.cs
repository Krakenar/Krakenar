using Krakenar.Contracts.Fields;
using Krakenar.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using FieldType = Krakenar.EntityFrameworkCore.Relational.Entities.FieldType;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class FieldTypeConfiguration : AggregateConfiguration<FieldType>, IEntityTypeConfiguration<FieldType>
{
  public override void Configure(EntityTypeBuilder<FieldType> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.FieldTypes.Table.Table ?? string.Empty, KrakenarDb.FieldTypes.Table.Schema);
    builder.HasKey(x => x.FieldTypeId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.RealmId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.DataType);
    builder.HasIndex(x => x.RelatedContentTypeUid);

    builder.Property(x => x.UniqueName).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.DataType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<DataType>());

    builder.HasOne(x => x.Realm).WithMany(x => x.FieldTypes)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.RelatedContentType).WithMany(x => x.RelatedFieldTypes)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.RelatedContentTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
