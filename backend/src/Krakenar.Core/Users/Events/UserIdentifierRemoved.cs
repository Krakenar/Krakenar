using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserIdentifierRemoved(Identifier Key) : DomainEvent;
