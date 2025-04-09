using Logitar.EventSourcing;

namespace Krakenar.Core.Roles.Events;

public record RoleCreated(UniqueName UniqueName) : DomainEvent;
