using Krakenar.Core.Tokens;
using Logitar.EventSourcing;

namespace Krakenar.Core.Realms.Events;

public record RealmSecretChanged(Secret Secret) : DomainEvent;
