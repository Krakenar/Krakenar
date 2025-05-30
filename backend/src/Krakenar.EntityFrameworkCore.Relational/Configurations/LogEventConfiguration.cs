using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class LogEventConfiguration : IEntityTypeConfiguration<LogEvent>
{
  public void Configure(EntityTypeBuilder<LogEvent> builder)
  {
    builder.ToTable(KrakenarDb.LogEvents.Table.Table ?? string.Empty, KrakenarDb.LogEvents.Table.Schema);
    builder.HasKey(x => x.LogEventId);

    builder.HasIndex(x => new { x.LogId, x.EventId }).IsUnique();
    builder.HasIndex(x => x.LogUid);

    builder.Property(x => x.EventId).HasMaxLength(EventId.MaximumLength);

    builder.HasOne(x => x.Log).WithMany(x => x.Events)
      .HasPrincipalKey(x => x.LogId).HasForeignKey(x => x.LogId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
