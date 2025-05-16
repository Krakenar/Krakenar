using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;

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

  public Content(ContentType contentType, ContentCreated @event)
  {
    Realm? realm = contentType.Realm;
    if (realm is not null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

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

  public void SetLocale(Language? language, ContentLocaleChanged @event)
  {
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
}
