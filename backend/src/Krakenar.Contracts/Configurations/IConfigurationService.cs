namespace Krakenar.Contracts.Configurations;

public interface IConfigurationService
{
  Task<Configuration> ReadAsync(CancellationToken cancellationToken = default);
  Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken = default);
}
