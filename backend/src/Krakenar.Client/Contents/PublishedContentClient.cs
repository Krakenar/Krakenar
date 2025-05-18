using Krakenar.Contracts;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Contents;

public class PublishedContentClient : BaseClient, IPublishedContentService
{
  protected virtual Uri Path { get; } = new("/api/published/contents", UriKind.Relative);

  public PublishedContentClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<PublishedContent?> ReadAsync(int? id, Guid? uid, PublishedContentKey? key, CancellationToken cancellationToken)
  {
    Dictionary<Guid, PublishedContent> publishedContents = new(capacity: 3);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      PublishedContent? publishedContent = (await GetAsync<PublishedContent>(uri, cancellationToken)).Value;
      if (publishedContent is not null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (uid.HasValue)
    {
      Uri uri = new($"{Path}/{uid}", UriKind.Relative);
      PublishedContent? publishedContent = (await GetAsync<PublishedContent>(uri, cancellationToken)).Value;
      if (publishedContent is not null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (key is not null)
    {
      Uri uri = new($"{Path}/types/{key.ContentType}/name:{key.UniqueName}?language={key.Language}", UriKind.Relative);
      PublishedContent? publishedContent = (await GetAsync<PublishedContent>(uri, cancellationToken)).Value;
      if (publishedContent is not null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (publishedContents.Count > 1)
    {
      throw TooManyResultsException<ContentType>.ExpectedSingle(publishedContents.Count);
    }

    return publishedContents.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = [];
    parameters["content_id"] = [payload.Content.Ids];
    parameters["content_uid"] = [payload.Content.Uids];
    parameters["language_id"] = [payload.Language.Ids];
    parameters["language_uid"] = [payload.Language.Uids];
    parameters["language_code"] = [payload.Language.Codes];
    parameters["language_default"] = [payload.Language.IsDefault];
    parameters["type_id"] = [payload.ContentType.Ids];
    parameters["type_uid"] = [payload.ContentType.Uids];
    parameters["type_name"] = [payload.ContentType.Names];
    parameters["search"] = payload.Search.Terms.Select(term => (object?)WebUtility.UrlEncode(term.Value)).ToList();
    parameters["search_operator"] = [payload.Search.Operator];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort.Field)).ToList();
    parameters["skip"] = [payload.Skip];
    parameters["limit"] = [payload.Limit];

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<PublishedContentLocale>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }
}
