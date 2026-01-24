using Krakenar.Contracts.Configurations;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Configurations.Queries;
using Logitar.CQRS;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations;

public class ConfigurationService : IConfigurationService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public ConfigurationService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<ConfigurationDto> ReadAsync(CancellationToken cancellationToken)
  {
    ReadConfiguration query = new();
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<ConfigurationDto> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    ReplaceConfiguration command = new(payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ConfigurationDto> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    UpdateConfiguration command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
