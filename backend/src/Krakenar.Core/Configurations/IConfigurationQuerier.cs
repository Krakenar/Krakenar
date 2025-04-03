using Krakenar.Contracts.Configurations;

namespace Krakenar.Core.Configurations;

public interface IConfigurationQuerier
{
  Task<ConfigurationModel?> ReadAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
