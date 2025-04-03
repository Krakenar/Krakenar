using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Localization;

public readonly struct LanguageId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public LanguageId(RealmId? realmId, Guid entityId)
  {
    // TODO(fpion): StreamId

    RealmId = realmId;
    EntityId = entityId;
  }
  public LanguageId(StreamId streamId)
  {
    StreamId = streamId;

    // TODO(fpion): RealmId
    // TODO(fpion): EntityId
  }
  public LanguageId(string value) : this(new StreamId(value))
  {
  }

  public static LanguageId NewId(RealmId? realmId) => new(realmId, Guid.NewGuid());

  public static bool operator ==(LanguageId left, LanguageId right) => left.Equals(right);
  public static bool operator !=(LanguageId left, LanguageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LanguageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
