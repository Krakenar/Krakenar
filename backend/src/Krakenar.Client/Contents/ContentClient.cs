using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Contents;

public class ContentClient : BaseClient, IContentClient
{
  protected virtual Uri Path { get; } = new("/api/contents", UriKind.Relative);

  public ContentClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<Content> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await CreateAsync(payload, context);
  }
  public virtual async Task<Content> CreateAsync(CreateContentPayload payload, RequestContext? context)
  {
    ApiResult<Content> result = await PostAsync<Content>(Path, payload, context);
    return result.Value ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, Path, payload);
  }

  public virtual async Task<Content?> DeleteAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await DeleteAsync(id, language, context);
  }
  public virtual async Task<Content?> DeleteAsync(Guid id, string? language, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}?language={language}", UriKind.Relative);
    return (await DeleteAsync<Content>(uri, context)).Value;
  }

  public virtual async Task<Content?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await PublishAllAsync(id, context);
  }
  public virtual async Task<Content?> PublishAllAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/publish/all", UriKind.Relative);
    return (await PatchAsync<Content>(uri, context)).Value;
  }

  public virtual async Task<Content?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await PublishAsync(id, language, context);
  }
  public virtual async Task<Content?> PublishAsync(Guid id, string? language, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/publish?language={language}", UriKind.Relative);
    return (await PatchAsync<Content>(uri, context)).Value;
  }

  public virtual async Task<Content?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await ReadAsync(id, context);
  }
  public virtual async Task<Content?> ReadAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<Content>(uri, context)).Value;
  }

  public virtual async Task<Content?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SaveLocaleAsync(id, payload, language, context);
  }
  public virtual async Task<Content?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}?language={language}", UriKind.Relative);
    return (await PutAsync<Content>(uri, payload, context)).Value;
  }

  public virtual async Task<SearchResults<ContentLocale>> SearchLocalesAsync(SearchContentLocalesPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SearchLocalesAsync(payload, context);
  }
  public virtual async Task<SearchResults<ContentLocale>> SearchLocalesAsync(SearchContentLocalesPayload payload, RequestContext? context)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["type"] = [payload.ContentTypeId];
    parameters["language"] = [payload.LanguageId];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<ContentLocale>>(uri, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchLocalesAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Content?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await UnpublishAllAsync(id, context);
  }
  public virtual async Task<Content?> UnpublishAllAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/unpublish/all", UriKind.Relative);
    return (await PatchAsync<Content>(uri, context)).Value;
  }

  public virtual async Task<Content?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await UnpublishAsync(id, language, context);
  }
  public virtual async Task<Content?> UnpublishAsync(Guid id, string? language, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/unpublish?language={language}", UriKind.Relative);
    return (await PatchAsync<Content>(uri, context)).Value;
  }

  public virtual async Task<Content?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await UpdateLocaleAsync(id, payload, language, context);
  }
  public virtual async Task<Content?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}?language={language}", UriKind.Relative);
    return (await PatchAsync<Content>(uri, payload, context)).Value;
  }
}
