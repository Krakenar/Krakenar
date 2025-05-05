using Krakenar.Core.Roles;
using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys.Events;

public record ApiKeyRoleAdded(RoleId RoleId) : DomainEvent;
