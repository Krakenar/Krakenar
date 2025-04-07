using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserEmailChanged(Email? Email) : DomainEvent;
