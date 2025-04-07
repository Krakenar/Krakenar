using Krakenar.Core.Roles;
using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserRoleAdded(RoleId RoleId) : DomainEvent;
