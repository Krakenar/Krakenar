﻿using Krakenar.Contracts.Actors;
using Krakenar.Core;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class ActorConfiguration : IEntityTypeConfiguration<ActorEntity>
{
  public void Configure(EntityTypeBuilder<ActorEntity> builder)
  {
    builder.ToTable(KrakenarDb.Actors.Table.Table ?? string.Empty, KrakenarDb.Actors.Table.Schema);
    builder.HasKey(x => x.ActorId);

    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => new { x.RealmId, x.Type, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.IsDeleted);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.EmailAddress);

    builder.Property(x => x.Key).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<ActorType>());
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.EmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.PictureUrl).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.Actors)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
