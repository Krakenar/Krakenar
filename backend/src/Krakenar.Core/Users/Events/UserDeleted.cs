using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserDeleted : DomainEvent, IDeleteEvent;
