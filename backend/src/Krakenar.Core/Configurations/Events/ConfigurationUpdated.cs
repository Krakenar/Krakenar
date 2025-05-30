using Krakenar.Core.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations.Events;

public record ConfigurationUpdated : DomainEvent
{
  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public LoggingSettings? LoggingSettings { get; set; }
}
