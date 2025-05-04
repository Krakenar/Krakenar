using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys;

public readonly struct ApiKeyId
{
  private const string EntityType = "ApiKey";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public ApiKeyId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public ApiKeyId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public ApiKeyId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(ApiKeyId left, ApiKeyId right) => left.Equals(right);
  public static bool operator !=(ApiKeyId left, ApiKeyId right) => !left.Equals(right);

  public static ApiKeyId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ApiKeyId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
