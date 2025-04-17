using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class RealmConfiguration : AggregateConfiguration<Realm>, IEntityTypeConfiguration<Realm>
{
  public override void Configure(EntityTypeBuilder<Realm> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Realms.Table.Table ?? string.Empty, KrakenarDb.Realms.Table.Schema);
    builder.HasKey(x => x.RealmId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.UniqueSlug);
    builder.HasIndex(x => x.UniqueSlugNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueSlug).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.UniqueSlugNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Secret).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.AllowedUniqueNameCharacters).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordHashingStrategy).HasMaxLength(byte.MaxValue);
  }
}
