using Logitar.EventSourcing;

namespace Krakenar.Core.Roles.Events;

public record RoleUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
