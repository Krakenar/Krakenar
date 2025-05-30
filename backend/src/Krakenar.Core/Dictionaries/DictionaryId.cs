﻿using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Dictionaries;

public readonly struct DictionaryId
{
  private const string EntityType = "Dictionary";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public DictionaryId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public DictionaryId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public DictionaryId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(DictionaryId left, DictionaryId right) => left.Equals(right);
  public static bool operator !=(DictionaryId left, DictionaryId right) => !left.Equals(right);

  public static DictionaryId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is DictionaryId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
