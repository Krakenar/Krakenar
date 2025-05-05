using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class ApiKeyConfiguration : AggregateConfiguration<ApiKey>, IEntityTypeConfiguration<ApiKey>
{
  public override void Configure(EntityTypeBuilder<ApiKey> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.ApiKeys.Table.Table ?? string.Empty, KrakenarDb.ApiKeys.Table.Schema);
    builder.HasKey(x => x.ApiKeyId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.ExpiresOn);
    builder.HasIndex(x => x.AuthenticatedOn);

    builder.Property(x => x.SecretHash).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Name).HasMaxLength(DisplayName.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.ApiKeys)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasMany(x => x.Roles).WithMany(x => x.ApiKeys).UsingEntity<ApiKeyRole>(joinBuilder =>
    {
      joinBuilder.ToTable(KrakenarDb.ApiKeyRoles.Table.Table ?? string.Empty, KrakenarDb.ApiKeyRoles.Table.Schema);
      joinBuilder.HasKey(x => new { x.ApiKeyId, x.RoleId });
    });
  }
}
