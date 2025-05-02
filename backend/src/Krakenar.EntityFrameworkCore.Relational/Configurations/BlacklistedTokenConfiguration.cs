using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedToken>
{
  public void Configure(EntityTypeBuilder<BlacklistedToken> builder)
  {
    builder.ToTable(KrakenarDb.TokenBlacklist.Table.Table ?? string.Empty, KrakenarDb.TokenBlacklist.Table.Schema);
    builder.HasKey(x => x.BlacklistedTokenId);

    builder.HasIndex(x => x.TokenId).IsUnique();
    builder.HasIndex(x => x.ExpiresOn);

    builder.Property(x => x.TokenId).HasMaxLength(byte.MaxValue);
  }
}
