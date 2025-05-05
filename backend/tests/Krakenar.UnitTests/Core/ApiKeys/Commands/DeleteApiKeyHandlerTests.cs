using Krakenar.Core.Realms;
using Moq;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteApiKeyHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly RealmId _realmId = RealmId.NewId();

  private readonly Mock<IApiKeyQuerier> _apiKeyQuerier = new();
  private readonly Mock<IApiKeyRepository> _apiKeyRepository = new();
  private readonly Mock<IApplicationContext> _applicationContext = new();

  private readonly DeleteApiKeyHandler _handler;

  private readonly ApiKey _apiKey;

  public DeleteApiKeyHandlerTests()
  {
    _handler = new(_apiKeyQuerier.Object, _apiKeyRepository.Object, _applicationContext.Object);

    Base64Password secret = new(Guid.NewGuid().ToString());
    DisplayName name = new("Test");
    _apiKey = new(secret, name, actorId: null, ApiKeyId.NewId(_realmId));
    _apiKeyRepository.Setup(x => x.LoadAsync(_apiKey.Id, _cancellationToken)).ReturnsAsync(_apiKey);
  }

  [Fact(DisplayName = "It should delete an existing API key.")]
  public async Task Given_Found_When_HandleAsync_Then_Deleted()
  {
    _applicationContext.SetupGet(x => x.RealmId).Returns(_realmId);

    _apiKeyRepository.Setup(x => x.LoadAsync(_apiKey.Id, _cancellationToken)).ReturnsAsync(_apiKey);

    ApiKeyDto model = new();
    _apiKeyQuerier.Setup(x => x.ReadAsync(_apiKey, _cancellationToken)).ReturnsAsync(model);

    DeleteApiKey command = new(_apiKey.EntityId);
    ApiKeyDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.True(_apiKey.IsDeleted);

    _apiKeyRepository.Verify(x => x.SaveAsync(_apiKey, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should return null when the API key could not be found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteApiKey command = new(_apiKey.EntityId);
    ApiKeyDto? apiKey = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Null(apiKey);
  }
}
