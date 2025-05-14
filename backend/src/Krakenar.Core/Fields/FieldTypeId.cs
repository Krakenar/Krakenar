using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Fields;

public readonly struct FieldTypeId
{
  private const string EntityType = "FieldType";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public FieldTypeId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public FieldTypeId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public FieldTypeId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(FieldTypeId left, FieldTypeId right) => left.Equals(right);
  public static bool operator !=(FieldTypeId left, FieldTypeId right) => !left.Equals(right);

  public static FieldTypeId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FieldTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
