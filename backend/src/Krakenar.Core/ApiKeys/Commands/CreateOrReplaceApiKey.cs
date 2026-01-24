using FluentValidation;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Core.ApiKeys.Validators;
using Krakenar.Core.Passwords;
using Krakenar.Core.Roles;
using Logitar.CQRS;
using Logitar.EventSourcing;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Commands;

public record CreateOrReplaceApiKey(Guid? Id, CreateOrReplaceApiKeyPayload Payload, long? Version) : ICommand<CreateOrReplaceApiKeyResult>;

/// <exception cref="RolesNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceApiKeyHandler : ICommandHandler<CreateOrReplaceApiKey, CreateOrReplaceApiKeyResult>
{
  protected virtual IApiKeyQuerier ApiKeyQuerier { get; }
  protected virtual IApiKeyRepository ApiKeyRepository { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IPasswordManager PasswordManager { get; }
  protected virtual IRoleManager RoleManager { get; }

  public CreateOrReplaceApiKeyHandler(
    IApiKeyQuerier apiKeyQuerier,
    IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    IRoleManager roleManager)
  {
    ApiKeyQuerier = apiKeyQuerier;
    ApiKeyRepository = apiKeyRepository;
    ApplicationContext = applicationContext;
    PasswordManager = passwordManager;
    RoleManager = roleManager;
  }

  public virtual async Task<CreateOrReplaceApiKeyResult> HandleAsync(CreateOrReplaceApiKey command, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKeyPayload payload = command.Payload;
    new CreateOrReplaceApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyId apiKeyId = ApiKeyId.NewId(ApplicationContext.RealmId);
    ApiKey? apiKey = null;
    if (command.Id.HasValue)
    {
      apiKeyId = new(command.Id.Value, apiKeyId.RealmId);
      apiKey = await ApiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    }

    DisplayName name = new(payload.Name);
    ActorId? actorId = ApplicationContext.ActorId;

    string? secretString = null;
    bool created = false;
    if (apiKey is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceApiKeyResult();
      }

      Password secret = PasswordManager.GenerateBase64(XApiKey.SecretLength, out secretString);
      apiKey = new(secret, name, actorId, apiKeyId);
      created = true;
    }

    ApiKey reference = (command.Version.HasValue
      ? await ApiKeyRepository.LoadAsync(apiKeyId, command.Version, cancellationToken)
      : null) ?? apiKey;

    if (reference.Name != name)
    {
      apiKey.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      apiKey.Description = description;
    }
    if (reference.ExpiresOn != payload.ExpiresOn)
    {
      apiKey.ExpiresOn = payload.ExpiresOn;
    }

    apiKey.SetCustomAttributes(payload.CustomAttributes, reference);

    await UpdateRolesAsync(payload, apiKey, reference, actorId, cancellationToken);

    apiKey.Update(actorId);
    await ApiKeyRepository.SaveAsync(apiKey, cancellationToken);

    ApiKeyDto dto = await ApiKeyQuerier.ReadAsync(apiKey, cancellationToken);
    if (secretString is not null)
    {
      dto.XApiKey = XApiKey.Encode(apiKey, secretString);
    }
    return new CreateOrReplaceApiKeyResult(dto, created);
  }

  protected virtual async Task UpdateRolesAsync(CreateOrReplaceApiKeyPayload payload, ApiKey apiKey, ApiKey reference, ActorId? actorId, CancellationToken cancellationToken)
  {
    if (payload.Roles.Count > 0)
    {
      IReadOnlyDictionary<string, Role> roles = await RoleManager.FindAsync(payload.Roles, nameof(payload.Roles), cancellationToken);

      HashSet<RoleId> roleIds = new(capacity: roles.Count);
      foreach (Role role in roles.Values)
      {
        roleIds.Add(role.Id);

        if (!reference.HasRole(role))
        {
          apiKey.AddRole(role, actorId);
        }
      }

      foreach (RoleId roleId in reference.Roles)
      {
        if (!roleIds.Contains(roleId))
        {
          apiKey.RemoveRole(roleId, actorId);
        }
      }
    }
  }
}
