using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class OneTimePasswordConfiguration : AggregateConfiguration<OneTimePassword>, IEntityTypeConfiguration<OneTimePassword>
{
  public override void Configure(EntityTypeBuilder<OneTimePassword> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.OneTimePasswords.Table.Table ?? string.Empty, KrakenarDb.OneTimePasswords.Table.Schema);
    builder.HasKey(x => x.OneTimePasswordId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.ExpiresOn);
    builder.HasIndex(x => x.MaximumAttempts);
    builder.HasIndex(x => x.AttemptCount);
    builder.HasIndex(x => x.HasValidationSucceeded);

    builder.Property(x => x.PasswordHash).HasMaxLength(byte.MaxValue);

    builder.HasOne(x => x.Realm).WithMany(x => x.OneTimePasswords)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.User).WithMany(x => x.OneTimePasswords)
      .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
