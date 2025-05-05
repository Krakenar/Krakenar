using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.ApiKeys;

public class ApiKeyClient : BaseClient, IApiKeyService
{
  protected virtual Uri Path { get; } = new("/api/keys", UriKind.Relative);

  public ApiKeyClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<ApiKey> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/authenticate", UriKind.Relative);
    return (await PatchAsync<ApiKey>(uri, payload, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(AuthenticateAsync), HttpMethod.Patch, uri, payload);
  }

  public virtual async Task<CreateOrReplaceApiKeyResult> CreateOrReplaceAsync(CreateOrReplaceApiKeyPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<ApiKey> result = id is null
      ? await PostAsync<ApiKey>(Path, payload, cancellationToken)
      : await PutAsync<ApiKey>(new Uri($"{Path}/{id}?version={version}", UriKind.Relative), payload, cancellationToken);
    return new CreateOrReplaceApiKeyResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<ApiKey?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<ApiKey>(uri, cancellationToken)).Value;
  }

  public virtual async Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<ApiKey>(uri, cancellationToken)).Value;
  }

  public virtual async Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["authenticated"] = [payload.HasAuthenticated];
    parameters["role"] = [payload.RoleId];
    if (payload.Status is not null)
    {
      parameters["expired"] = [payload.Status.IsExpired];
      parameters["moment"] = [payload.Status.Moment];
    }
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<ApiKey>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<ApiKey?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<ApiKey>(uri, payload, cancellationToken)).Value;
  }
}
