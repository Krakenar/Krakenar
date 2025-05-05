using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords;

public readonly struct OneTimePasswordId
{
  private const string EntityType = "OneTimePassword";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public OneTimePasswordId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public OneTimePasswordId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public OneTimePasswordId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(OneTimePasswordId left, OneTimePasswordId right) => left.Equals(right);
  public static bool operator !=(OneTimePasswordId left, OneTimePasswordId right) => !left.Equals(right);

  public static OneTimePasswordId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is OneTimePasswordId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
