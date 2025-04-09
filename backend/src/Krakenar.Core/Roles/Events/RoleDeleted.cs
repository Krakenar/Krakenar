using Logitar.EventSourcing;

namespace Krakenar.Core.Roles.Events;

public record RoleDeleted : DomainEvent, IDeleteEvent;
