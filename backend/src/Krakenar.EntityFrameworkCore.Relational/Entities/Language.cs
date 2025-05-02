using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Language : Aggregate, ISegregatedEntity
{
  public int LanguageId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public bool IsDefault { get; private set; }

  public int LCID { get; private set; }
  public string Code { get; private set; } = string.Empty;
  public string CodeNormalized
  {
    get => Helper.Normalize(Code);
    private set { }
  }
  public string DisplayName { get; private set; } = string.Empty;
  public string EnglishName { get; private set; } = string.Empty;
  public string NativeName { get; private set; } = string.Empty;

  public Dictionary? Dictionary { get; private set; }

  public Language(Realm? realm, LanguageCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Id = new LanguageId(@event.StreamId).EntityId;

    IsDefault = @event.IsDefault;

    SetLocale(@event.Locale);
  }

  private Language() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipDictionary: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipDictionary)
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    if (!skipDictionary && Dictionary is not null)
    {
      actorIds.AddRange(Dictionary.GetActorIds(skipLanguage: true));
    }
    return actorIds.ToList().AsReadOnly();
  }

  public void SetDefault(LanguageSetDefault @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void SetLocale(LanguageLocaleChanged @event)
  {
    Update(@event);

    SetLocale(@event.Locale);
  }

  private void SetLocale(Locale locale)
  {
    LCID = locale.Culture.LCID;
    Code = locale.Code;
    DisplayName = locale.Culture.DisplayName;
    EnglishName = locale.Culture.EnglishName;
    NativeName = locale.Culture.NativeName;
  }

  public override string ToString() => $"{DisplayName} ({Code}) | {base.ToString()}";
}
