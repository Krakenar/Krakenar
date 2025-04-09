using Krakenar.Core.Roles.Events;

namespace Krakenar.Core.Roles;

public interface IRoleService
{
  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
}

public class RoleService : IRoleService
{
  protected virtual IRoleQuerier RoleQuerier { get; }
  protected virtual IRoleRepository RoleRepository { get; }

  public RoleService(IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    RoleQuerier = roleQuerier;
    RoleRepository = roleRepository;
  }

  public virtual async Task SaveAsync(Role role, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = role.Changes.Any(change => change is RoleCreated || change is RoleUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      RoleId? conflictId = await RoleQuerier.FindIdAsync(role.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(role.Id))
      {
        throw new UniqueNameAlreadyUsedException(role, conflictId.Value);
      }
    }

    await RoleRepository.SaveAsync(role, cancellationToken);
  }
}
