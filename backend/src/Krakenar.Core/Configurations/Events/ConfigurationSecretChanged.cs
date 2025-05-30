using Krakenar.Core.Tokens;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations.Events;

public record ConfigurationSecretChanged(Secret Secret) : DomainEvent;
