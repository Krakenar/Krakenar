using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Templates;

public readonly struct TemplateId
{
  private const string EntityType = "Template";

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public TemplateId(Guid entityId, RealmId? realmId = null)
  {
    StreamId = IdHelper.Construct(EntityType, entityId, realmId);

    EntityId = entityId;
    RealmId = realmId;
  }
  public TemplateId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<Guid, RealmId?> values = IdHelper.Deconstruct(streamId, EntityType);
    EntityId = values.Item1;
    RealmId = values.Item2;
  }
  public TemplateId(string value) : this(new StreamId(value))
  {
  }

  public static bool operator ==(TemplateId left, TemplateId right) => left.Equals(right);
  public static bool operator !=(TemplateId left, TemplateId right) => !left.Equals(right);

  public static TemplateId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TemplateId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
