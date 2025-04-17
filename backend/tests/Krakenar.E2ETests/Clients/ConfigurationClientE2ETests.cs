using Krakenar.Client.Configurations;
using Krakenar.Contracts.Configurations;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ConfigurationClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public ConfigurationClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "It should read the configuration.")]
  public async Task Given_Client_When_ReadAsync_Then_ConfigurationRead()
  {
    using HttpClient httpClient = new();
    ConfigurationClient client = new(httpClient, KrakenarSettings);

    Configuration configuration = await client.ReadAsync(_cancellationToken);
    Assert.True(configuration.Version > 0);
  }
}
