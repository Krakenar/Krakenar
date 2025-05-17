using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Contents;

public class ContentClient : BaseClient, IContentService
{
  protected virtual Uri Path { get; } = new("/api/contents", UriKind.Relative);

  public ContentClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<Content> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken)
  {
    ApiResult<Content> result = await PostAsync<Content>(Path, payload, cancellationToken);
    return result.Value ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, Path, payload);
  }

  public virtual async Task<Content?> DeleteAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}?language={language}", UriKind.Relative);
    return (await DeleteAsync<Content>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Content?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<Content>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Content?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}?language={language}", UriKind.Relative);
    return (await PutAsync<Content>(uri, payload, cancellationToken)).Value;
  }

  public virtual async Task<SearchResults<ContentLocale>> SearchLocalesAsync(SearchContentLocalesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["type"] = [payload.ContentTypeId];
    parameters["language"] = [payload.LanguageId];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<ContentLocale>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchLocalesAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Content?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}?language={language}", UriKind.Relative);
    return (await PatchAsync<Content>(uri, payload, cancellationToken)).Value;
  }
}
