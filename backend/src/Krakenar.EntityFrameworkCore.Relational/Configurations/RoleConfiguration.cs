using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public class RoleConfiguration : AggregateConfiguration<Role>, IEntityTypeConfiguration<Role>
{
  public override void Configure(EntityTypeBuilder<Role> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Roles.Table.Table!, KrakenarDb.Roles.Table.Schema);
    builder.HasKey(x => x.RoleId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.RealmId, x.UniqueName }).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.Roles)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
