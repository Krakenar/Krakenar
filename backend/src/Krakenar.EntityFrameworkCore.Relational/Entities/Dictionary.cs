using Krakenar.Core.Dictionaries;
using Krakenar.Core.Dictionaries.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Dictionary : Aggregate, ISegregatedEntity
{
  public int DictionaryId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public Language? Language { get; private set; }
  public int LanguageId { get; private set; }

  public int EntryCount { get; private set; }
  public string? Entries { get; private set; }

  public Dictionary(Language language, DictionaryCreated @event) : base(@event)
  {
    Realm = language.Realm;
    RealmId = language.Realm?.RealmId;
    RealmUid = language.Realm?.Id;

    Id = new DictionaryId(@event.StreamId).EntityId;

    Language = language;
    LanguageId = language.LanguageId;
  }

  private Dictionary() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    if (Language is not null)
    {
      actorIds.AddRange(Language.GetActorIds());
    }
    return actorIds.ToList().AsReadOnly();
  }

  public void SetLanguage(Language language, DictionaryLanguageChanged @event)
  {
    Update(@event);

    Language = language;
    LanguageId = language.LanguageId;
  }

  public void Update(DictionaryUpdated @event)
  {
    base.Update(@event);

    Dictionary<string, string> entries = GetEntries();
    foreach (KeyValuePair<Core.Identifier, string?> entry in @event.Entries)
    {
      if (entry.Value is null)
      {
        entries.Remove(entry.Key.Value);
      }
      else
      {
        entries[entry.Key.Value] = entry.Value;
      }
    }
    SetEntries(entries);
  }

  public Dictionary<string, string> GetEntries()
  {
    return (Entries is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Entries)) ?? [];
  }
  private void SetEntries(Dictionary<string, string> entries)
  {
    EntryCount = entries.Count;
    Entries = entries.Count < 1 ? null : JsonSerializer.Serialize(entries);
  }
}
