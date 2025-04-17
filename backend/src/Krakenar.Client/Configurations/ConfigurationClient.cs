using Krakenar.Contracts.Configurations;

namespace Krakenar.Client.Configurations;

public class ConfigurationClient : BaseClient, IConfigurationService
{
  protected virtual Uri Path { get; } = new("/api/configuration", UriKind.Relative);

  public ConfigurationClient(IKrakenarSettings settings) : base(settings)
  {
  }

  public virtual async Task<Configuration> ReadAsync(CancellationToken cancellationToken)
  {
    return (await GetAsync<Configuration>(Path, cancellationToken)).Value ?? throw new NotImplementedException(); // TODO(fpion): implement
  }

  public virtual async Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    Uri uri = new Uri($"{Path}?version={version}", UriKind.Relative);
    return (await PutAsync<Configuration>(uri, payload, cancellationToken)).Value ?? throw new NotImplementedException(); // TODO(fpion): implement
  }

  public virtual async Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return (await PatchAsync<Configuration>(Path, payload, cancellationToken)).Value ?? throw new NotImplementedException(); // TODO(fpion): implement
  }
}
