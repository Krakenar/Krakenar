using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords.Events;

public record OneTimePasswordDeleted : DomainEvent, IDeleteEvent;
