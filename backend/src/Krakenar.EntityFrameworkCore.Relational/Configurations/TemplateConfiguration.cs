using Krakenar.Core;
using Krakenar.Core.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Template;

namespace Krakenar.EntityFrameworkCore.Relational.Configurations;

public sealed class TemplateConfiguration : AggregateConfiguration<TemplateEntity>, IEntityTypeConfiguration<TemplateEntity>
{
  public const int ContentTypeMaximumLength = 16;

  public override void Configure(EntityTypeBuilder<TemplateEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(KrakenarDb.Templates.Table.Table ?? string.Empty, KrakenarDb.Templates.Table.Schema);
    builder.HasKey(x => x.TemplateId);

    builder.HasIndex(x => new { x.RealmId, x.Id }).IsUnique();
    builder.HasIndex(x => x.RealmUid);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.RealmId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.ContentType);

    builder.Property(x => x.UniqueName).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Subject).HasMaxLength(Subject.MaximumLength);
    builder.Property(x => x.ContentType).HasMaxLength(ContentTypeMaximumLength);

    builder.HasOne(x => x.Realm).WithMany(x => x.Templates)
      .HasPrincipalKey(x => x.RealmId).HasForeignKey(x => x.RealmId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
