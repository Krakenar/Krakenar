using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Roles;

public readonly struct RoleId
{
  private const string EntityType = "Role";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public RoleId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public RoleId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }

  public static bool operator ==(RoleId left, RoleId right) => left.Equals(right);
  public static bool operator !=(RoleId left, RoleId right) => !left.Equals(right);

  public static RoleId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RoleId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
