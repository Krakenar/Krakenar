using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Users;

public readonly struct UserId
{
  private const string EntityType = "User";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public UserId(RealmId? realmId, Guid entityId)
  {
    StreamId = IdHelper.Construct(realmId, EntityType, entityId);

    RealmId = realmId;
    EntityId = entityId;
  }
  public UserId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<RealmId?, Guid> components = IdHelper.Deconstruct(streamId, EntityType);
    RealmId = components.Item1;
    EntityId = components.Item2;
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
