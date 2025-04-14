using Krakenar.Core.Caching;
using Moq;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadConfigurationHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICacheService> _cacheService = new();

  private readonly ReadConfigurationHandler _handler;

  public ReadConfigurationHandlerTests()
  {
    _handler = new(_cacheService.Object);
  }

  [Fact(DisplayName = "It should read the configuration.")]
  public async Task Given_Query_When_HandleAsync_Then_Read()
  {
    ConfigurationDto configuration = new();
    _cacheService.SetupGet(x => x.Configuration).Returns(configuration);

    ReadConfiguration query = new();
    ConfigurationDto result = await _handler.HandleAsync(query, _cancellationToken);

    Assert.Same(configuration, result);
  }
}
