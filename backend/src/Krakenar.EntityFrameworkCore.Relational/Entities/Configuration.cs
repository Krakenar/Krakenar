using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public class Configuration
{
  public int ConfigurationId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; private set; } = string.Empty;

  public long Version { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public Configuration(string key, object value, DomainEvent @event)
  {
    Key = key;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(value, @event);
  }

  private Configuration()
  {
  }

  public void Update(object value, DomainEvent @event)
  {
    Value = value.ToString() ?? string.Empty;

    Version = @event.Version;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Configuration configuration && configuration.Key == Key;
  public override int GetHashCode() => Key.GetHashCode();
  public override string ToString() => $"{GetType()} (Key={Key})";
}
