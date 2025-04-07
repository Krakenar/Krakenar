using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations.Events;

public record ConfigurationUpdated : DomainEvent
{
  public Secret? Secret { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public LoggingSettings? LoggingSettings { get; set; }
}
