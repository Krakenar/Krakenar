using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Roles;
using Krakenar.Core.ApiKeys.Validators;
using Krakenar.Core.Roles;
using Logitar.EventSourcing;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;
using Role = Krakenar.Core.Roles.Role;

namespace Krakenar.Core.ApiKeys.Commands;

public record UpdateApiKey(Guid Id, UpdateApiKeyPayload Payload) : ICommand<ApiKeyDto?>;

/// <exception cref="RolesNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateApiKeyHandler : ICommandHandler<UpdateApiKey, ApiKeyDto?>
{
  protected virtual IApiKeyQuerier ApiKeyQuerier { get; }
  protected virtual IApiKeyRepository ApiKeyRepository { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IRoleManager RoleManager { get; }

  public UpdateApiKeyHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext, IRoleManager roleManager)
  {
    ApiKeyQuerier = apiKeyQuerier;
    ApiKeyRepository = apiKeyRepository;
    ApplicationContext = applicationContext;
    RoleManager = roleManager;
  }

  public virtual async Task<ApiKeyDto?> HandleAsync(UpdateApiKey command, CancellationToken cancellationToken)
  {
    UpdateApiKeyPayload payload = command.Payload;
    new UpdateApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyId apiKeyId = new(command.Id, ApplicationContext.RealmId);
    ApiKey? apiKey = await ApiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    if (apiKey is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      apiKey.Name = new DisplayName(payload.Name);
    }
    if (payload.Description is not null)
    {
      apiKey.Description = Description.TryCreate(payload.Description.Value);
    }
    if (payload.ExpiresOn.HasValue)
    {
      apiKey.ExpiresOn = payload.ExpiresOn.Value;
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      apiKey.SetCustomAttribute(new Identifier(customAttribute.Key), customAttribute.Value);
    }

    if (payload.Roles.Count > 0)
    {
      IReadOnlyDictionary<string, Role> roles = await RoleManager.FindAsync(payload.Roles.Select(role => role.Role), nameof(payload.Roles), cancellationToken);
      foreach (RoleChange change in payload.Roles)
      {
        Role role = roles[change.Role];
        switch (change.Action)
        {
          case CollectionAction.Add:
            apiKey.AddRole(role, actorId);
            break;
          case CollectionAction.Remove:
            apiKey.RemoveRole(role, actorId);
            break;
        }
      }
    }

    apiKey.Update(actorId);
    await ApiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await ApiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
