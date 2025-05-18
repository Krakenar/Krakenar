using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Content : Aggregate, ISegregatedEntity
{
  public int ContentId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public ContentType? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }

  public List<ContentLocale> Locales { get; private set; } = [];
  public List<PublishedContent> PublishedContents { get; private set; } = [];

  public Content(ContentType contentType, ContentCreated @event) : base(@event)
  {
    Realm = contentType.Realm;
    RealmId = contentType.RealmId;
    RealmUid = contentType.RealmUid;

    Id = new ContentId(@event.StreamId).EntityId;

    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    ContentTypeUid = contentType.Id;

    ContentLocale invariant = new(this, language: null, @event.Invariant, @event);
    Locales.Add(invariant);
  }

  private Content() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipLocales: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipLocales)
  {
    List<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    if (ContentType is not null)
    {
      actorIds.AddRange(ContentType.GetActorIds());
    }
    if (!skipLocales)
    {
      foreach (ContentLocale locale in Locales)
      {
        actorIds.AddRange(locale.GetActorIds(skipContent: true));
      }
    }
    return actorIds.AsReadOnly();
  }

  public ContentLocale? Publish(ContentLocalePublished @event)
  {
    Update(@event);

    ContentLocale? locale = Locales.SingleOrDefault(l => l.LanguageUid == @event.LanguageId?.EntityId);
    if (locale is null)
    {
      return null;
    }

    locale.Publish(@event);
    return locale;
  }

  public ContentLocale? RemoveLocale(ContentLocaleRemoved @event)
  {
    Update(@event);

    ContentLocale? locale = Locales.SingleOrDefault(l => l.LanguageUid == @event.LanguageId.EntityId);
    if (locale is not null)
    {
      Locales.Remove(locale);
    }

    return locale;
  }

  public void SetLocale(Language? language, ContentLocaleChanged @event)
  {
    Update(@event);

    ContentLocale? locale = Locales.SingleOrDefault(l => language is null ? l.LanguageId is null : l.LanguageId == language.LanguageId);
    if (locale is null)
    {
      locale = new ContentLocale(this, language, @event.Locale, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event.Locale, @event);
    }
  }

  public ContentLocale? Unpublish(ContentLocaleUnpublished @event)
  {
    Update(@event);

    ContentLocale? locale = Locales.SingleOrDefault(l => l.LanguageUid == @event.LanguageId?.EntityId);
    if (locale is null)
    {
      return null;
    }

    locale.Unpublish(@event);
    return locale;
  }
}
