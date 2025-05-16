using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class ContentLocale
{
  public int ContentLocaleId { get; private set; }

  public ContentType? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }

  public Content? Content { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }

  public Language? Language { get; private set; }
  public int? LanguageId { get; private set; }
  public Guid? LanguageUid { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public long Version { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ContentLocale(Content content, Language? language, Core.Contents.ContentLocale locale, DomainEvent @event)
  {
    ContentType = content.ContentType;
    ContentTypeId = content.ContentTypeId;
    ContentTypeUid = content.ContentTypeUid;

    Content = content;
    ContentId = content.ContentId;
    ContentUid = content.Id;

    if (language is not null)
    {
      Language = language;
      LanguageId = language.LanguageId;
      LanguageUid = language.Id;
    }

    Update(locale, @event);

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();
  }

  private ContentLocale()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipContent: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipContent)
  {
    List<ActorId> actorIds = [];
    if (ContentType is not null)
    {
      actorIds.AddRange(ContentType.GetActorIds());
    }
    if (!skipContent && Content is not null)
    {
      actorIds.AddRange(Content.GetActorIds(skipLocales: true));
    }
    if (Language is not null)
    {
      actorIds.AddRange(Language.GetActorIds());
    }
    if (CreatedBy is not null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy is not null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    return actorIds.AsReadOnly();
  }

  public void Update(Core.Contents.ContentLocale locale, DomainEvent @event)
  {
    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Version = @event.Version;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is ContentLocale locale && locale.ContentLocaleId == ContentLocaleId;
  public override int GetHashCode() => ContentLocaleId.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (ContentLocaleId={ContentLocaleId})";
}
