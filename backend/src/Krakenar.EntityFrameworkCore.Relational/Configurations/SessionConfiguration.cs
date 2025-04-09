using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class SessionConfiguration : AggregateConfiguration<Session>, IEntityTypeConfiguration<Session>
{
  public override void Configure(EntityTypeBuilder<Session> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Sessions.Table.Table ?? string.Empty, KrakenarDb.Sessions.Table.Schema);
    builder.HasKey(x => x.SessionId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.Id);
    builder.HasIndex(x => x.IsPersistent);
    builder.HasIndex(x => x.SignedOutBy);
    builder.HasIndex(x => x.SignedOutOn);
    builder.HasIndex(x => x.IsActive);

    builder.Property(x => x.SecretHash).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.SignedOutBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.Sessions)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.User).WithMany(x => x.Sessions)
      .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
