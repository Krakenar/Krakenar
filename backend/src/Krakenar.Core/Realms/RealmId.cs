using Logitar.EventSourcing;

namespace Krakenar.Core.Realms;

public readonly struct RealmId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId(Guid id)
  {
    StreamId = new StreamId(id); // TODO(fpion): AggregateType
  }
  public RealmId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public RealmId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static RealmId NewId() => new(Guid.NewGuid());

  public Guid ToGuid() => StreamId.ToGuid();

  public static bool operator ==(RealmId left, RealmId right) => left.Equals(right);
  public static bool operator !=(RealmId left, RealmId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RealmId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
