using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class LogExceptionConfiguration : IEntityTypeConfiguration<LogException>
{
  public void Configure(EntityTypeBuilder<LogException> builder)
  {
    builder.ToTable(KrakenarDb.LogExceptions.Table.Table ?? string.Empty, KrakenarDb.LogExceptions.Table.Schema);
    builder.HasKey(x => x.LogExceptionId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.LogUid);
    builder.HasIndex(x => x.Type);

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue);

    builder.HasOne(x => x.Log).WithMany(x => x.Exceptions)
      .HasPrincipalKey(x => x.LogId).HasForeignKey(x => x.LogId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
