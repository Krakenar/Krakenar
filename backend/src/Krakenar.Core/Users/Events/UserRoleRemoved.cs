using Krakenar.Core.Roles;
using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserRoleRemoved(RoleId RoleId) : DomainEvent;
