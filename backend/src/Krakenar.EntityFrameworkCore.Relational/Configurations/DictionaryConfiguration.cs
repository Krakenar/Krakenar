using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class DictionaryConfiguration : AggregateConfiguration<Dictionary>, IEntityTypeConfiguration<Dictionary>
{
  public override void Configure(EntityTypeBuilder<Dictionary> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Dictionaries.Table.Table ?? string.Empty, KrakenarDb.Dictionaries.Table.Schema);
    builder.HasKey(x => x.DictionaryId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => new { x.RealmId, x.LanguageId }).IsUnique();
    builder.HasIndex(x => x.EntryCount);

    builder.HasOne(x => x.Realm).WithMany(x => x.Dictionaries)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Language).WithOne(x => x.Dictionary)
      .HasPrincipalKey<Language>(x => x.LanguageId).HasForeignKey<Dictionary>(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
