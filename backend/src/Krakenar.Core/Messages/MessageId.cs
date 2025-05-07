using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Messages;

public readonly struct MessageId
{
  private const string EntityType = "Message";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public MessageId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public MessageId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public MessageId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(MessageId left, MessageId right) => left.Equals(right);
  public static bool operator !=(MessageId left, MessageId right) => !left.Equals(right);

  public static MessageId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is MessageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
