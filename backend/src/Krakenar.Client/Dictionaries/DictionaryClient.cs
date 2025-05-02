using Krakenar.Contracts;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Dictionaries;

public class DictionaryClient : BaseClient, IDictionaryService
{
  protected virtual Uri Path { get; } = new("/api/dictionaries", UriKind.Relative);

  public DictionaryClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<CreateOrReplaceDictionaryResult> CreateOrReplaceAsync(CreateOrReplaceDictionaryPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<Dictionary> result = id is null
      ? await PostAsync<Dictionary>(Path, payload, cancellationToken)
      : await PutAsync<Dictionary>(new Uri($"{Path}/{id}?version={version}", UriKind.Relative), payload, cancellationToken);
    return new CreateOrReplaceDictionaryResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<Dictionary?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<Dictionary>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Dictionary?> ReadAsync(Guid? id, Guid? languageId, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Dictionary> dictionaries = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Dictionary? dictionary = (await GetAsync<Dictionary>(uri, cancellationToken)).Value;
      if (dictionary is not null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (languageId.HasValue)
    {
      Uri uri = new($"{Path}/language:{languageId}", UriKind.Relative);
      Dictionary? dictionary = (await GetAsync<Dictionary>(uri, cancellationToken)).Value;
      if (dictionary is not null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw TooManyResultsException<Dictionary>.ExpectedSingle(dictionaries.Count);
    }

    return dictionaries.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Dictionary>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<Dictionary>(uri, payload, cancellationToken)).Value;
  }
}
