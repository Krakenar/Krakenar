using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations.Events;

public record ConfigurationInitialized(Secret Secret, UniqueNameSettings UniqueNameSettings, PasswordSettings PasswordSettings, LoggingSettings LoggingSettings) : DomainEvent;
