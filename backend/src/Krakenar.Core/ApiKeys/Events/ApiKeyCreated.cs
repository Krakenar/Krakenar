using Krakenar.Core.Passwords;
using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys.Events;

public record ApiKeyCreated(Password Secret, DisplayName Name) : DomainEvent;
