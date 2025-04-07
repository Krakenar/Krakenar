using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserIdentifierChanged(Identifier Key, CustomIdentifier Value) : DomainEvent;
