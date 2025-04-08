using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Localization;

public class Language : AggregateRoot
{
  public new LanguageId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public bool IsDefault { get; private set; }

  private Locale? _locale = null;
  public Locale Locale => _locale ?? throw new InvalidOperationException("The language has not been initialized.");

  public Language() : base()
  {
  }

  public Language(Locale locale, bool isDefault = false, ActorId? actorId = null, LanguageId? languageId = null)
    : base((languageId ?? LanguageId.NewId()).StreamId)
  {
    Raise(new LanguageCreated(isDefault, locale), actorId);
  }
  protected virtual void Handle(LanguageCreated @event)
  {
    IsDefault = @event.IsDefault;

    _locale = @event.Locale;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new LanguageDeleted(), actorId);
    }
  }

  public void SetDefault(bool isDefault = true, ActorId? actorId = null)
  {
    if (IsDefault != isDefault)
    {
      Raise(new LanguageSetDefault(isDefault), actorId);
    }
  }
  protected virtual void Handle(LanguageSetDefault @event)
  {
    IsDefault = @event.IsDefault;
  }

  public void SetLocale(Locale locale, ActorId? actorId = null)
  {
    if (_locale != locale)
    {
      Raise(new LanguageLocaleChanged(locale), actorId);
    }
  }
  protected virtual void Handle(LanguageLocaleChanged @event)
  {
    _locale = @event.Locale;
  }

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
