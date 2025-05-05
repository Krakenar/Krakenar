using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Commands;

public record DeleteApiKey(Guid Id) : ICommand<ApiKeyDto?>;

public class DeleteApiKeyHandler : ICommandHandler<DeleteApiKey, ApiKeyDto?>
{
  protected virtual IApiKeyQuerier ApiKeyQuerier { get; }
  protected virtual IApiKeyRepository ApiKeyRepository { get; }
  protected virtual IApplicationContext ApplicationContext { get; }

  public DeleteApiKeyHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext)
  {
    ApiKeyQuerier = apiKeyQuerier;
    ApiKeyRepository = apiKeyRepository;
    ApplicationContext = applicationContext;
  }

  public virtual async Task<ApiKeyDto?> HandleAsync(DeleteApiKey command, CancellationToken cancellationToken)
  {
    ApiKeyId apiKeyId = new(command.Id, ApplicationContext.RealmId);
    ApiKey? apiKey = await ApiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    if (apiKey is null)
    {
      return null;
    }
    ApiKeyDto dto = await ApiKeyQuerier.ReadAsync(apiKey, cancellationToken);

    apiKey.Delete(ApplicationContext.ActorId);
    await ApiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return dto;
  }
}
