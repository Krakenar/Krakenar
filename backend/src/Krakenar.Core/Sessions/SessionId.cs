using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions;

public readonly struct SessionId
{
  private const string EntityType = "Session";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public SessionId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public SessionId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public SessionId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(SessionId left, SessionId right) => left.Equals(right);
  public static bool operator !=(SessionId left, SessionId right) => !left.Equals(right);

  public static SessionId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SessionId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
