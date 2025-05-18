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
  private ContentStatus? _invariantStatus;
  public ContentLocale Invariant => _invariant ?? throw new InvalidOperationException("The content has not been initialized yet.");

  private readonly Dictionary<LanguageId, ContentLocale> _locales = [];
  private readonly Dictionary<LanguageId, ContentStatus> _localeStatuses = [];
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

  public bool IsInvariantPublished() => _invariantStatus.HasValue;
  public bool IsLocalePublished(Language language) => IsLocalePublished(language.Id);
  public bool IsLocalePublished(LanguageId languageId) => _localeStatuses.ContainsKey(languageId);
  public bool IsPublished(LanguageId? languageId) => languageId.HasValue ? IsLocalePublished(languageId.Value) : IsInvariantPublished();

  public void Publish(ActorId? actorId = null)
  {
    PublishInvariant(actorId);

    foreach (LanguageId languageId in _locales.Keys)
    {
      PublishLocale(languageId, actorId);
    }
  }
  public void PublishInvariant(ActorId? actorId = null)
  {
    if (_invariantStatus != ContentStatus.Latest)
    {
      Raise(new ContentLocalePublished(LanguageId: null), actorId);
    }
  }
  public bool PublishLocale(Language language, ActorId? actorId = null) => PublishLocale(language.Id, actorId);
  public bool PublishLocale(LanguageId languageId, ActorId? actorId = null)
  {
    if (!HasLocale(languageId))
    {
      return false;
    }
    else if (!_localeStatuses.TryGetValue(languageId, out ContentStatus status) || status != ContentStatus.Latest)
    {
      Raise(new ContentLocalePublished(languageId), actorId);
    }

    return true;
  }
  protected virtual void Handle(ContentLocalePublished @event)
  {
    if (@event.LanguageId.HasValue)
    {
      _localeStatuses[@event.LanguageId.Value] = ContentStatus.Latest;
    }
    else
    {
      _invariantStatus = ContentStatus.Latest;
    }
  }

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

    _localeStatuses.Remove(@event.LanguageId);
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

      if (_localeStatuses.TryGetValue(@event.LanguageId.Value, out ContentStatus status) && status == ContentStatus.Latest)
      {
        _localeStatuses[@event.LanguageId.Value] = ContentStatus.Published;
      }
    }
    else
    {
      _invariant = @event.Locale;

      if (_invariantStatus == ContentStatus.Latest)
      {
        _invariantStatus = ContentStatus.Published;
      }
    }
  }

  public ContentLocale? TryGetLocale(Language language) => TryGetLocale(language.Id);
  public ContentLocale? TryGetLocale(LanguageId languageId) => _locales.TryGetValue(languageId, out ContentLocale? locale) ? locale : null;

  public void Unpublish(ActorId? actorId = null)
  {
    UnpublishInvariant(actorId);

    foreach (LanguageId languageId in _locales.Keys)
    {
      UnpublishLocale(languageId, actorId);
    }
  }
  public void UnpublishInvariant(ActorId? actorId = null)
  {
    if (IsInvariantPublished())
    {
      Raise(new ContentLocaleUnpublished(LanguageId: null), actorId);
    }
  }
  public bool UnpublishLocale(Language language, ActorId? actorId = null) => UnpublishLocale(language.Id, actorId);
  public bool UnpublishLocale(LanguageId languageId, ActorId? actorId = null)
  {
    if (!HasLocale(languageId))
    {
      return false;
    }
    else if (IsPublished(languageId))
    {
      Raise(new ContentLocaleUnpublished(languageId), actorId);
    }

    return true;
  }
  protected virtual void Handle(ContentLocaleUnpublished @event)
  {
    if (@event.LanguageId.HasValue)
    {
      _localeStatuses.Remove(@event.LanguageId.Value);
    }
    else
    {
      _invariantStatus = null;
    }
  }

  public override string ToString() => $"{Invariant.DisplayName?.Value ?? Invariant.UniqueName.Value} | {base.ToString()}";
}
