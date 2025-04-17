using Krakenar.Core;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomAttributeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.CustomAttribute;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class CustomAttributeConfiguration : IEntityTypeConfiguration<CustomAttributeEntity>
{
  public void Configure(EntityTypeBuilder<CustomAttributeEntity> builder)
  {
    builder.ToTable(KrakenarDb.CustomAttributes.Table.Table ?? string.Empty, KrakenarDb.CustomAttributes.Table.Schema);
    builder.HasKey(x => x.CustomAttributeId);

    builder.HasIndex(x => new { x.Entity, x.Key }).IsUnique();
    builder.HasIndex(x => x.Key);
    builder.HasIndex(x => x.ValueShortened);

    builder.Property(x => x.Entity).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.Key).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.ValueShortened).HasMaxLength(CustomAttributeEntity.ValueShortenedLength);
  }
}
