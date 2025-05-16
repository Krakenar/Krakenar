using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class ContentTypeConfiguration : AggregateConfiguration<ContentType>, IEntityTypeConfiguration<ContentType>
{
  public override void Configure(EntityTypeBuilder<ContentType> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.ContentTypes.Table.Table ?? string.Empty, KrakenarDb.ContentTypes.Table.Schema);
    builder.HasKey(x => x.ContentTypeId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.IsInvariant);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.RealmId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.FieldCount);

    builder.Property(x => x.UniqueName).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.ContentTypes)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
