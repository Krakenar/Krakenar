using Krakenar.Contracts.Actors;
using Krakenar.Core;
using Krakenar.Core.Configurations.Queries;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Configurations;

[Trait(Traits.Category, Categories.Integration)]
public class ConfigurationIntegrationTests : IntegrationTests
{
  private readonly IQueryHandler<ReadConfiguration, ConfigurationDto> _readConfiguration;

  public ConfigurationIntegrationTests()
  {
    _readConfiguration = ServiceProvider.GetRequiredService<IQueryHandler<ReadConfiguration, ConfigurationDto>>();
  }

  [Fact(DisplayName = "It should read the configuration.")]
  public async Task Given_Configuration_When_Read_Then_Configuration()
  {
    ReadConfiguration query = new();
    ConfigurationDto configuration = await _readConfiguration.HandleAsync(query);

    Assert.Equal(1, configuration.Version);
    Assert.Equal(ActorType.User, configuration.CreatedBy.Type);
    Assert.Equal(ActorType.User, configuration.UpdatedBy.Type);
    Assert.Equal(configuration.CreatedOn, configuration.UpdatedOn);
    Assert.Equal(configuration.UpdatedOn.AsUniversalTime(), DateTime.UtcNow, TimeSpan.FromSeconds(1));
    Assert.Null(configuration.Secret);
  }
}
