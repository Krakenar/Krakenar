using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class UserIdentifierConfiguration : IdentifierConfiguration<UserIdentifier>, IEntityTypeConfiguration<UserIdentifier>
{
  public override void Configure(EntityTypeBuilder<UserIdentifier> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.UserIdentifiers.Table.Table ?? string.Empty, KrakenarDb.UserIdentifiers.Table.Schema);
    builder.HasKey(x => x.UserIdentifierId);

    builder.HasIndex(x => new { x.UserId, x.Key }).IsUnique();
    builder.HasIndex(x => x.UserUid);
    builder.HasIndex(x => x.Key);
    builder.HasIndex(x => x.Value);

    builder.HasOne(x => x.Realm).WithMany(x => x.UserIdentifiers)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.User).WithMany(x => x.Identifiers)
      .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
