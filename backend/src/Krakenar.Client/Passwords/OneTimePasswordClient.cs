using Krakenar.Contracts.Passwords;

namespace Krakenar.Client.Passwords;

public class OneTimePasswordClient : BaseClient, IOneTimePasswordService
{
  protected virtual Uri Path { get; } = new("/api/one-time-passwords", UriKind.Relative);

  public OneTimePasswordClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    return (await PostAsync<OneTimePassword>(Path, payload, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, Path, payload);
  }

  public virtual async Task<OneTimePassword?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<OneTimePassword>(uri, cancellationToken)).Value;
  }

  public virtual async Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PutAsync<OneTimePassword>(uri, payload, cancellationToken)).Value;
  }
}
