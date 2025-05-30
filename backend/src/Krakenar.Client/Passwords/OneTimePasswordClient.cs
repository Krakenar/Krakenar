using Krakenar.Contracts.Passwords;

namespace Krakenar.Client.Passwords;

public class OneTimePasswordClient : BaseClient, IOneTimePasswordClient
{
  protected virtual Uri Path { get; } = new("/api/one-time-passwords", UriKind.Relative);

  public OneTimePasswordClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await CreateAsync(payload, context);
  }
  public virtual async Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, RequestContext? context)
  {
    return (await PostAsync<OneTimePassword>(Path, payload, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, Path, payload);
  }

  public virtual async Task<OneTimePassword?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await ReadAsync(id, context);
  }
  public virtual async Task<OneTimePassword?> ReadAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<OneTimePassword>(uri, context)).Value;
  }

  public virtual async Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await ValidateAsync(id, payload, context);
  }
  public virtual async Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PutAsync<OneTimePassword>(uri, payload, context)).Value;
  }
}
