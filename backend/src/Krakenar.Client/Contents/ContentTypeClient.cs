using Krakenar.Contracts;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Contents;

public class ContentTypeClient : BaseClient, IContentTypeService
{
  protected virtual Uri Path { get; } = new("/api/contents/types", UriKind.Relative);

  public ContentTypeClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<CreateOrReplaceContentTypeResult> CreateOrReplaceAsync(CreateOrReplaceContentTypePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<ContentType> result = id is null
      ? await PostAsync<ContentType>(Path, payload, cancellationToken)
      : await PutAsync<ContentType>(new Uri($"{Path}/{id}?version={version}", UriKind.Relative), payload, cancellationToken);
    return new CreateOrReplaceContentTypeResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<ContentType?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<ContentType>(uri, cancellationToken)).Value;
  }

  public virtual async Task<ContentType?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ContentType> roles = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      ContentType? role = (await GetAsync<ContentType>(uri, cancellationToken)).Value;
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/name:{uniqueName}", UriKind.Relative);
      ContentType? role = (await GetAsync<ContentType>(uri, cancellationToken)).Value;
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw TooManyResultsException<ContentType>.ExpectedSingle(roles.Count);
    }

    return roles.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<ContentType>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["invariant"] = [payload.IsInvariant];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<ContentType>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<ContentType?> UpdateAsync(Guid id, UpdateContentTypePayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<ContentType>(uri, payload, cancellationToken)).Value;
  }
}
