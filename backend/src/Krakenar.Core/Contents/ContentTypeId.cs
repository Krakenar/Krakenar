using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

public readonly struct ContentTypeId
{
  private const string EntityType = "ContentType";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public ContentTypeId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public ContentTypeId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public ContentTypeId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(ContentTypeId left, ContentTypeId right) => left.Equals(right);
  public static bool operator !=(ContentTypeId left, ContentTypeId right) => !left.Equals(right);

  public static ContentTypeId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
