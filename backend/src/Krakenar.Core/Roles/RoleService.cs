using Krakenar.Core.Realms;
using Krakenar.Core.Roles.Events;

namespace Krakenar.Core.Roles;

public interface IRoleService
{
  Task<IReadOnlyDictionary<string, Role>> FindAsync(IEnumerable<string> roles, string propertyName, CancellationToken cancellationToken = default);

  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
}

public class RoleService : IRoleService
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IRoleQuerier RoleQuerier { get; }
  protected virtual IRoleRepository RoleRepository { get; }

  public RoleService(IApplicationContext applicationContext, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    ApplicationContext = applicationContext;
    RoleQuerier = roleQuerier;
    RoleRepository = roleRepository;
  }

  public virtual async Task<IReadOnlyDictionary<string, Role>> FindAsync(IEnumerable<string> idOrUniqueNames, string propertyName, CancellationToken cancellationToken)
  {
    idOrUniqueNames = idOrUniqueNames.Distinct();
    int count = idOrUniqueNames.Count();
    if (count < 1)
    {
      return new Dictionary<string, Role>().AsReadOnly();
    }

    IReadOnlyDictionary<Guid, string> uniqueNameByIds = await RoleQuerier.ListUniqueNameByIdsAsync(cancellationToken);
    Dictionary<string, Guid> idByUniqueNames = uniqueNameByIds.ToDictionary(x => Normalize(x.Value), x => x.Key);
    HashSet<RoleId> roleIds = new(capacity: count);
    HashSet<string> missingIds = new(capacity: count);
    RealmId? realmId = ApplicationContext.RealmId;
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      string normalized = Normalize(idOrUniqueName);
      if ((Guid.TryParse(idOrUniqueName, out Guid id) || idByUniqueNames.TryGetValue(normalized, out id)) && uniqueNameByIds.ContainsKey(id))
      {
        roleIds.Add(new RoleId(id, realmId));
      }
      else
      {
        missingIds.Add(idOrUniqueName);
      }
    }

    if (missingIds.Count > 0)
    {
      throw new RolesNotFoundException(realmId, missingIds, propertyName);
    }

    IReadOnlyCollection<Role> roles = await RoleRepository.LoadAsync(roleIds, cancellationToken);
    Dictionary<Guid, Role> rolesById = new(capacity: roles.Count);
    Dictionary<string, Role> rolesByUniqueName = new(capacity: roles.Count);
    foreach (Role role in roles)
    {
      rolesById[role.EntityId] = role;
      rolesByUniqueName[Normalize(role.UniqueName.Value)] = role;
    }

    Dictionary<string, Role> foundRoles = new(capacity: roles.Count);
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      if (!Guid.TryParse(idOrUniqueName, out Guid id) || !rolesById.TryGetValue(id, out Role? role))
      {
        role = rolesByUniqueName[Normalize(idOrUniqueName)];
      }
      foundRoles[idOrUniqueName] = role;
    }
    return foundRoles.AsReadOnly();
  }
  protected virtual string Normalize(string value) => value.Trim().ToUpperInvariant();

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
