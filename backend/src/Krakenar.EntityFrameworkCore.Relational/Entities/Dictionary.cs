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
  public List<DictionaryEntry> Entries { get; private set; } = [];

  public Dictionary(Language language, DictionaryCreated @event) : base(@event)
  {
    Realm = language.Realm;
    RealmId = language.RealmId;
    RealmUid = language.RealmUid;

    Id = new DictionaryId(@event.StreamId).EntityId;

    Language = language;
    LanguageId = language.LanguageId;
  }

  private Dictionary() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipLanguage: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipLanguage)
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    if (!skipLanguage && Language is not null)
    {
      actorIds.AddRange(Language.GetActorIds(skipDictionary: true));
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

    EntryCount = Entries.Count;
  }
}
