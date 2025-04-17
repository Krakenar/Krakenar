using Krakenar.Contracts;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Localization;

public class LanguageClient : BaseClient, ILanguageService
{
  protected virtual Uri Path { get; } = new("/api/Languages", UriKind.Relative);

  public LanguageClient(IKrakenarSettings settings) : base(settings)
  {
  }

  public virtual async Task<CreateOrReplaceLanguageResult> CreateOrReplaceAsync(CreateOrReplaceLanguagePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<Language> result = id is null
      ? await PostAsync<Language>(Path, payload, cancellationToken)
      : await PutAsync<Language>(new Uri($"{Path}/{id}?version={version}"), payload, cancellationToken);
    return new CreateOrReplaceLanguageResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<Language?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<Language>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Language?> ReadAsync(Guid? id, string? locale, bool isDefault, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Language> languages = new(capacity: 3);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Language? language = (await GetAsync<Language>(uri, cancellationToken)).Value;
      if (language is not null)
      {
        languages[language.Id] = language;
      }
    }

    if (!string.IsNullOrWhiteSpace(locale))
    {
      Uri uri = new($"{Path}/locale:{locale}", UriKind.Relative);
      Language? language = (await GetAsync<Language>(uri, cancellationToken)).Value;
      if (language is not null)
      {
        languages[language.Id] = language;
      }
    }

    if (isDefault)
    {
      Uri uri = new($"{Path}/default", UriKind.Relative);
      Language? language = (await GetAsync<Language>(uri, cancellationToken)).Value;
      if (language is not null)
      {
        languages[language.Id] = language;
      }
    }

    if (languages.Count > 1)
    {
      throw TooManyResultsException<Language>.ExpectedSingle(languages.Count);
    }

    return languages.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<Language>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Language>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Language?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}/default", UriKind.Relative);
    return (await PatchAsync<Language>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Language?> UpdateAsync(Guid id, UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<Language>(uri, payload, cancellationToken)).Value;
  }
}
