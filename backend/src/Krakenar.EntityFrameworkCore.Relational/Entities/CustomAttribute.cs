using Logitar;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class CustomAttribute
{
  public const int ValueShortenedLength = byte.MaxValue;

  public int CustomAttributeId { get; private set; }

  public string Entity { get; private set; } = string.Empty;

  public string Key { get; private set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public string ValueShortened
  {
    get => Value.Truncate(ValueShortenedLength);
    private set { }
  }

  public CustomAttribute(string entity, string key)
  {
    Entity = entity;

    Key = key;
  }

  private CustomAttribute()
  {
  }

  public override bool Equals(object? obj) => obj is CustomAttribute customAttribute && customAttribute.Entity == Entity && customAttribute.Key == Key;
  public override int GetHashCode() => HashCode.Combine(Entity, Key);
  public override string ToString() => $"{GetType()} (Entity={Entity}, Key={Key})";
}
