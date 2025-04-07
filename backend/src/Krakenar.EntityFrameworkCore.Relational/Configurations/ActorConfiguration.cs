using Krakenar.Contracts.Actors;
using Krakenar.Core;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public class ActorConfiguration : IEntityTypeConfiguration<ActorEntity>
{
  public void Configure(EntityTypeBuilder<ActorEntity> builder)
  {
    builder.ToTable(KrakenarDb.Actors.Table.Table!, KrakenarDb.Actors.Table.Schema);
    builder.HasKey(x => x.ActorId);

    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => new { x.Type, x.Id }).IsUnique(); // TODO(fpion): won't work accross realms!
    builder.HasIndex(x => x.IsDeleted);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.EmailAddress);

    builder.Property(x => x.Key).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<ActorType>());
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.EmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.PictureUrl).HasMaxLength(Url.MaximumLength);
  }
}
