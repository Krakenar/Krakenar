using Krakenar.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DictionaryEntry = Krakenar.EntityFrameworkCore.Relational.Entities.DictionaryEntry;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class DictionaryEntryConfiguration : IEntityTypeConfiguration<DictionaryEntry>
{
  public void Configure(EntityTypeBuilder<DictionaryEntry> builder)
  {
    builder.ToTable(KrakenarDb.DictionaryEntries.Table.Table ?? string.Empty, KrakenarDb.DictionaryEntries.Table.Schema);
    builder.HasKey(x => x.DictionaryEntryId);

    builder.HasIndex(x => new { x.DictionaryId, x.Key }).IsUnique();
    builder.HasIndex(x => x.Key);
    builder.HasIndex(x => x.ValueShortened);

    builder.Property(x => x.Key).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.ValueShortened).HasMaxLength(DictionaryEntry.ValueShortenedLength);

    builder.HasOne(x => x.Dictionary).WithMany(x => x.Entries)
      .HasPrincipalKey(x => x.DictionaryId).HasForeignKey(x => x.DictionaryId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
