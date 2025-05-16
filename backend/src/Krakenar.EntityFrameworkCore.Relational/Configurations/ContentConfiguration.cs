using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class ContentConfiguration : AggregateConfiguration<Content>, IEntityTypeConfiguration<Content>
{
  public override void Configure(EntityTypeBuilder<Content> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Contents.Table.Table ?? string.Empty, KrakenarDb.Contents.Table.Schema);
    builder.HasKey(x => x.ContentId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.ContentTypeUid);

    builder.HasOne(x => x.Realm).WithMany(x => x.Contents)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.ContentType).WithMany(x => x.Contents)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
