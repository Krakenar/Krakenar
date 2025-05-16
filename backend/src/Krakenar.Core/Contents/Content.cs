using Krakenar.Core.Contents.Events;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

public class Content : AggregateRoot
{
  public new ContentId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public ContentTypeId ContentTypeId { get; private set; }

  private ContentLocale? _invariant = null;
  public ContentLocale Invariant => _invariant ?? throw new InvalidOperationException("The content has not been initialized yet.");

  private readonly Dictionary<LanguageId, ContentLocale> _locales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocale> Locales => _locales.AsReadOnly();

  public Content() : base()
  {
  }

  public Content(ContentType contentType, ContentLocale invariant, ActorId? actorId = null, ContentId? contentId = null)
    : base((contentId ?? ContentId.NewId(contentType.RealmId)).StreamId)
  {
    if (RealmId != contentType.RealmId)
    {
      throw new RealmMismatchException(RealmId, contentType.RealmId, nameof(contentType));
    }

    Raise(new ContentCreated(contentType.Id, invariant), actorId);
  }
  protected virtual void Handle(ContentCreated @event)
  {
    ContentTypeId = @event.ContentTypeId;

    _invariant = @event.Invariant;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ContentDeleted(), actorId);
    }
  }

  public ContentLocale FindLocale(Language language) => FindLocale(language.Id);
  public ContentLocale FindLocale(LanguageId languageId) => TryGetLocale(languageId) ?? throw new InvalidOperationException($"The content locale 'LanguageId={languageId}' could not be found.");

  public bool HasLocale(Language language) => HasLocale(language.Id);
  public bool HasLocale(LanguageId languageId) => _locales.ContainsKey(languageId);

  public bool RemoveLocale(Language language, ActorId? actorId = null) => RemoveLocale(language.Id, actorId);
  public bool RemoveLocale(LanguageId languageId, ActorId? actorId = null)
  {
    if (!HasLocale(languageId))
    {
      return false;
    }

    Raise(new ContentLocaleRemoved(languageId), actorId);
    return true;
  }
  protected virtual void Handle(ContentLocaleRemoved @event)
  {
    _locales.Remove(@event.LanguageId);
  }

  public void SetInvariant(ContentLocale invariant, ActorId? actorId = null)
  {
    if (_invariant != invariant)
    {
      Raise(new ContentLocaleChanged(LanguageId: null, invariant), actorId);
    }
  }
  public void SetLocale(Language language, ContentLocale locale, ActorId? actorId = null) => SetLocale(language.Id, locale, actorId);
  public void SetLocale(LanguageId languageId, ContentLocale locale, ActorId? actorId = null)
  {
    if (RealmId != languageId.RealmId)
    {
      throw new RealmMismatchException(RealmId, languageId.RealmId, nameof(languageId));
    }

    ContentLocale? existingLocale = TryGetLocale(languageId);
    if (existingLocale is null || existingLocale != locale)
    {
      Raise(new ContentLocaleChanged(languageId, locale), actorId);
    }
  }
  protected virtual void Handle(ContentLocaleChanged @event)
  {
    if (@event.LanguageId.HasValue)
    {
      _locales[@event.LanguageId.Value] = @event.Locale;
    }
    else
    {
      _invariant = @event.Locale;
    }
  }

  public ContentLocale? TryGetLocale(Language language) => TryGetLocale(language.Id);
  public ContentLocale? TryGetLocale(LanguageId languageId) => _locales.TryGetValue(languageId, out ContentLocale? locale) ? locale : null;

  public override string ToString() => $"{Invariant.DisplayName?.Value ?? Invariant.UniqueName.Value} | {base.ToString()}";
}
