using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class ConfigurationConfiguration : IEntityTypeConfiguration<Configuration>
{
  public void Configure(EntityTypeBuilder<Configuration> builder)
  {
    builder.ToTable(KrakenarDb.Configuration.Table.Table ?? string.Empty, KrakenarDb.Configuration.Table.Schema);
    builder.HasKey(x => x.ConfigurationId);

    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => x.Value);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.Key).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Value).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
  }
}
