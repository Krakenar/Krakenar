using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Roles.Validators;
using Logitar.EventSourcing;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Commands;

public record UpdateRole(Guid Id, UpdateRolePayload Payload) : ICommand<RoleDto?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateRoleHandler : ICommandHandler<UpdateRole, RoleDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IRoleManager RoleManager { get; }
  protected virtual IRoleQuerier RoleQuerier { get; }
  protected virtual IRoleRepository RoleRepository { get; }

  public UpdateRoleHandler(
    IApplicationContext applicationContext,
    IRoleManager roleManager,
    IRoleQuerier roleQuerier,
    IRoleRepository roleRepository)
  {
    ApplicationContext = applicationContext;
    RoleManager = roleManager;
    RoleQuerier = roleQuerier;
    RoleRepository = roleRepository;
  }

  public virtual async Task<RoleDto?> HandleAsync(UpdateRole command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    UpdateRolePayload payload = command.Payload;
    new UpdateRoleValidator(uniqueNameSettings).ValidateAndThrow(payload);

    RoleId roleId = new(command.Id, ApplicationContext.RealmId);
    Role? role = await RoleRepository.LoadAsync(roleId, cancellationToken);
    if (role is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      role.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      role.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      role.Description = Description.TryCreate(payload.Description.Value);
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      role.SetCustomAttribute(key, customAttribute.Value);
    }

    role.Update(actorId);
    await RoleManager.SaveAsync(role, cancellationToken);

    return await RoleQuerier.ReadAsync(role, cancellationToken);
  }
}
