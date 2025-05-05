using Krakenar.Core.Users;
using Logitar.EventSourcing;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Commands;

public record DeleteRole(Guid Id) : ICommand<RoleDto?>;

public class DeleteRoleHandler : ICommandHandler<DeleteRole, RoleDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IRoleQuerier RoleQuerier { get; }
  protected virtual IRoleRepository RoleRepository { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public DeleteRoleHandler(
    IApplicationContext applicationContext,
    IRoleQuerier roleQuerier,
    IRoleRepository roleRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    ApplicationContext = applicationContext;
    RoleQuerier = roleQuerier;
    RoleRepository = roleRepository;
    UserQuerier = userQuerier;
    UserRepository = userRepository;
  }

  public virtual async Task<RoleDto?> HandleAsync(DeleteRole command, CancellationToken cancellationToken)
  {
    RoleId roleId = new(command.Id, ApplicationContext.RealmId);
    Role? role = await RoleRepository.LoadAsync(roleId, cancellationToken);
    if (role is null)
    {
      return null;
    }
    RoleDto dto = await RoleQuerier.ReadAsync(role, cancellationToken);

    ActorId? actorId = ApplicationContext.ActorId;

    IReadOnlyCollection<UserId> userIds = await UserQuerier.FindIdsAsync(role.Id, cancellationToken);
    if (userIds.Count > 0)
    {
      IReadOnlyCollection<User> users = await UserRepository.LoadAsync(userIds, cancellationToken);
      foreach (User user in users)
      {
        user.RemoveRole(role, actorId);
      }
      await UserRepository.SaveAsync(users, cancellationToken);
    }

    role.Delete(actorId);
    await RoleRepository.SaveAsync(role, cancellationToken);

    return dto;
  }
}
