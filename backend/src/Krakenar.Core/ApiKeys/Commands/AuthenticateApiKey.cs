using FluentValidation;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Core.ApiKeys.Validators;
using Logitar;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Commands;

public record AuthenticateApiKey(AuthenticateApiKeyPayload Payload) : ICommand<ApiKeyDto>, ISensitiveActivity
{
  public IActivity Anonymize()
  {
    AuthenticateApiKey clone = this.DeepClone();
    clone.Payload.XApiKey = Payload.XApiKey.Mask();
    return clone;
  }
}

/// <exception cref="ApiKeyIsExpiredException"></exception>
/// <exception cref="ApiKeyNotFoundException"></exception>
/// <exception cref="IncorrectApiKeySecretException"></exception>
/// <exception cref="InvalidApiKeyException"></exception>
/// <exception cref="ValidationException"></exception>
public class AuthenticateApiKeyHandler : ICommandHandler<AuthenticateApiKey, ApiKeyDto>
{
  protected virtual IApiKeyQuerier ApiKeyQuerier { get; }
  protected virtual IApiKeyRepository ApiKeyRepository { get; }
  protected virtual IApplicationContext ApplicationContext { get; }

  public AuthenticateApiKeyHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext)
  {
    ApiKeyQuerier = apiKeyQuerier;
    ApiKeyRepository = apiKeyRepository;
    ApplicationContext = applicationContext;
  }

  public virtual async Task<ApiKeyDto> HandleAsync(AuthenticateApiKey command, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyPayload payload = command.Payload;
    new AuthenticateApiKeyValidator().ValidateAndThrow(payload);

    XApiKey xApiKey;
    try
    {
      xApiKey = XApiKey.Decode(payload.XApiKey, ApplicationContext.RealmId);
    }
    catch (Exception innerException)
    {
      throw new InvalidApiKeyException(payload.XApiKey, nameof(payload.XApiKey), innerException);
    }

    ApiKey apiKey = await ApiKeyRepository.LoadAsync(xApiKey.ApiKeyId, cancellationToken)
      ?? throw new ApiKeyNotFoundException(xApiKey.ApiKeyId, nameof(payload.XApiKey));

    apiKey.Authenticate(xApiKey.Secret, ApplicationContext.ActorId);

    await ApiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await ApiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
