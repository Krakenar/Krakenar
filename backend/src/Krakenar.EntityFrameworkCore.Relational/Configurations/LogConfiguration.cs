using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class LogConfiguration : IEntityTypeConfiguration<Log>
{
  public void Configure(EntityTypeBuilder<Log> builder)
  {
    builder.ToTable(KrakenarDb.Logs.Table.Table ?? string.Empty, KrakenarDb.Logs.Table.Schema);
    builder.HasKey(x => x.LogId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.CorrelationId);
    builder.HasIndex(x => x.Method);
    builder.HasIndex(x => x.OperationType);
    builder.HasIndex(x => x.OperationName);
    builder.HasIndex(x => x.ActivityType);
    builder.HasIndex(x => x.StatusCode);
    builder.HasIndex(x => x.IsCompleted);
    builder.HasIndex(x => x.Level);
    builder.HasIndex(x => x.HasErrors);
    builder.HasIndex(x => x.StartedOn);
    builder.HasIndex(x => x.EndedOn);
    builder.HasIndex(x => x.Duration);
    builder.HasIndex(x => x.RealmId);
    builder.HasIndex(x => x.ActorId);
    builder.HasIndex(x => x.ApiKeyId);
    builder.HasIndex(x => x.SessionId);
    builder.HasIndex(x => x.UserId);

    builder.Property(x => x.CorrelationId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Method).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Destination).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Source).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.OperationType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.OperationName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ActivityType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Level).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<LogLevel>());
    builder.Property(x => x.RealmId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.ActorId).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.ApiKeyId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.SessionId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.UserId).HasMaxLength(StreamId.MaximumLength);
  }
}
