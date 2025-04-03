using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Users;

public readonly struct UserId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public UserId(RealmId? realmId, Guid entityId)
  {
    // TODO(fpion): StreamId

    RealmId = realmId;
    EntityId = entityId;
  }
  public UserId(StreamId streamId)
  {
    StreamId = streamId;

    // TODO(fpion): RealmId
    // TODO(fpion): EntityId
  }
  public UserId(string value) : this(new StreamId(value))
  {
  }

  public static UserId NewId(RealmId? realmId) => new(realmId, Guid.NewGuid());

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is UserId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
