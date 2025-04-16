using Krakenar.Contracts.Configurations;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Configurations.Queries;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations;

public class ConfigurationService : IConfigurationService
{
  protected virtual IQueryHandler<ReadConfiguration, ConfigurationDto> ReadConfiguration { get; }
  protected virtual ICommandHandler<ReplaceConfiguration, ConfigurationDto> ReplaceConfiguration { get; }
  protected virtual ICommandHandler<UpdateConfiguration, ConfigurationDto> UpdateConfiguration { get; }

  public ConfigurationService(
    IQueryHandler<ReadConfiguration, ConfigurationDto> readConfiguration,
    ICommandHandler<ReplaceConfiguration, ConfigurationDto> replaceConfiguration,
    ICommandHandler<UpdateConfiguration, ConfigurationDto> updateConfiguration)
  {
    ReadConfiguration = readConfiguration;
    ReplaceConfiguration = replaceConfiguration;
    UpdateConfiguration = updateConfiguration;
  }

  public virtual async Task<ConfigurationDto> ReadAsync(CancellationToken cancellationToken)
  {
    ReadConfiguration query = new();
    return await ReadConfiguration.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<ConfigurationDto> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    ReplaceConfiguration command = new(payload, version);
    return await ReplaceConfiguration.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ConfigurationDto> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    UpdateConfiguration command = new(payload);
    return await UpdateConfiguration.HandleAsync(command, cancellationToken);
  }
}
