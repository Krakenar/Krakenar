using Logitar.EventSourcing;

namespace Krakenar.Core.Localization;

public readonly struct LanguageId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public LanguageId(Guid id)
  {
    StreamId = new StreamId(id);
  }
  public LanguageId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static LanguageId NewId() => new(Guid.NewGuid());

  public static bool operator ==(LanguageId left, LanguageId right) => left.Equals(right);
  public static bool operator !=(LanguageId left, LanguageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LanguageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
