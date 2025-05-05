using Krakenar.Core.Roles;
using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys.Events;

public record ApiKeyRoleRemoved(RoleId RoleId) : DomainEvent;
