using Krakenar.Core.Dictionaries.Events;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Dictionaries;

public class Dictionary : AggregateRoot, ICustomizable
{
  private DictionaryUpdated _updated = new();
  private bool HasUpdates => _updated.Entries.Count > 0;

  public new DictionaryId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public LanguageId LanguageId { get; private set; }

  private readonly Dictionary<Identifier, string> _entries = [];
  public IReadOnlyDictionary<Identifier, string> Entries => _entries.AsReadOnly();

  public Dictionary() : base()
  {
  }

  public Dictionary(Language language, ActorId? actorId = null, DictionaryId? dictionaryId = null)
    : base((dictionaryId ?? DictionaryId.NewId()).StreamId)
  {
    // TODO(fpion): ensure language is in the same realm

    Raise(new DictionaryCreated(language.Id), actorId);
  }
  protected virtual void Handle(DictionaryCreated @event)
  {
    LanguageId = @event.LanguageId;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new DictionaryDeleted(), actorId);
    }
  }

  public void RemoveEntry(Identifier key)
  {
    if (_entries.Remove(key))
    {
      _updated.Entries[key] = null;
    }
  }

  public void SetEntry(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveEntry(key);
    }
    else
    {
      value = value.Trim();
      if (!_entries.TryGetValue(key, out string? existingValue) || existingValue != value)
      {
        _entries[key] = value;
        _updated.Entries[key] = value;
      }
    }
  }

  public void SetLanguage(Language language, ActorId? actorId = null)
  {
    if (LanguageId != language.Id)
    {
      Raise(new DictionaryLanguageChanged(language.Id), actorId);
    }
  }
  protected virtual void Handle(DictionaryLanguageChanged @event)
  {
    LanguageId = @event.LanguageId;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new DictionaryUpdated();
    }
  }
  protected virtual void Handle(DictionaryUpdated @event)
  {
    foreach (KeyValuePair<Identifier, string?> entry in @event.Entries)
    {
      if (entry.Value is null)
      {
        _entries.Remove(entry.Key);
      }
      else
      {
        _entries[entry.Key] = entry.Value;
      }
    }
  }
}
