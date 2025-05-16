using Krakenar.Core;
using Krakenar.Core.Fields;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class FieldDefinitionConfiguration : IEntityTypeConfiguration<Entities.FieldDefinition>
{
  public void Configure(EntityTypeBuilder<Entities.FieldDefinition> builder)
  {
    builder.ToTable(KrakenarDb.FieldDefinitions.Table.Table ?? string.Empty, KrakenarDb.FieldDefinitions.Table.Schema);
    builder.HasKey(x => x.FieldDefinitionId);

    builder.HasIndex(x => x.ContentTypeUid);
    builder.HasIndex(x => new { x.ContentTypeId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.ContentTypeId, x.Order }).IsUnique();
    builder.HasIndex(x => x.FieldTypeUid);
    builder.HasIndex(x => x.IsInvariant);
    builder.HasIndex(x => x.IsRequired);
    builder.HasIndex(x => x.IsIndexed);
    builder.HasIndex(x => x.IsUnique);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.ContentTypeId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Placeholder);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Placeholder).HasMaxLength(Placeholder.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.ContentType).WithMany(x => x.FieldDefinitions)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldType).WithMany(x => x.FieldDefinitions)
      .HasPrincipalKey(x => x.FieldTypeId).HasForeignKey(x => x.FieldTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
