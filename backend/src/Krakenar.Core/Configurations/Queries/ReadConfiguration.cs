using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations.Queries;

public record ReadConfiguration : IQuery<ConfigurationDto>;

public class ReadConfigurationHandler : IQueryHandler<ReadConfiguration, ConfigurationDto>
{
  protected virtual IConfigurationQuerier ConfigurationQuerier { get; }

  public ReadConfigurationHandler(IConfigurationQuerier configurationQuerier)
  {
    ConfigurationQuerier = configurationQuerier;
  }

  public async Task<ConfigurationDto> HandleAsync(ReadConfiguration _, CancellationToken cancellationToken)
  {
    return await ConfigurationQuerier.ReadAsync(cancellationToken);
  }
}
