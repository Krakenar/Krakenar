using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Senders;

public readonly struct SenderId
{
  private const string EntityType = "Sender";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public SenderId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public SenderId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public SenderId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(SenderId left, SenderId right) => left.Equals(right);
  public static bool operator !=(SenderId left, SenderId right) => !left.Equals(right);

  public static SenderId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SenderId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
