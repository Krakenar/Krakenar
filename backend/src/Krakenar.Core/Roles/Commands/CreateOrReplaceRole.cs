using FluentValidation;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Roles.Validators;
using Logitar.EventSourcing;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Commands;

public record CreateOrReplaceRoleResult(RoleDto? Role = null, bool Created = false);

public record CreateOrReplaceRole(Guid? Id, CreateOrReplaceRolePayload Payload, long? Version) : ICommand<CreateOrReplaceRoleResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceRoleHandler : ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IRoleQuerier RoleQuerier { get; }
  protected virtual IRoleRepository RoleRepository { get; }
  protected virtual IRoleService RoleService { get; }

  public CreateOrReplaceRoleHandler(IApplicationContext applicationContext, IRoleQuerier roleQuerier, IRoleRepository roleRepository, IRoleService roleService)
  {
    ApplicationContext = applicationContext;
    RoleQuerier = roleQuerier;
    RoleRepository = roleRepository;
    RoleService = roleService;
  }

  public virtual async Task<CreateOrReplaceRoleResult> HandleAsync(CreateOrReplaceRole command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    CreateOrReplaceRolePayload payload = command.Payload;
    new CreateOrReplaceRoleValidator(uniqueNameSettings).ValidateAndThrow(payload);

    RoleId roleId = RoleId.NewId(ApplicationContext.RealmId);
    Role? role = null;
    if (command.Id.HasValue)
    {
      roleId = new(command.Id.Value, roleId.RealmId);
      role = await RoleRepository.LoadAsync(roleId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (role is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceRoleResult();
      }

      role = new(uniqueName, actorId, roleId);
      created = true;
    }

    Role reference = (command.Version.HasValue
      ? await RoleRepository.LoadAsync(roleId, command.Version, cancellationToken)
      : null) ?? role;

    if (reference.UniqueName != uniqueName)
    {
      role.SetUniqueName(uniqueName, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      role.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      role.Description = description;
    }

    role.SetCustomAttributes(payload.CustomAttributes, reference);

    role.Update(actorId);
    await RoleService.SaveAsync(role, cancellationToken);

    RoleDto dto = await RoleQuerier.ReadAsync(role, cancellationToken);
    return new CreateOrReplaceRoleResult(dto, created);
  }
}
