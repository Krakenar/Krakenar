using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Realms;

public class RealmClient : BaseClient, IRealmService
{
  protected virtual Uri Path { get; } = new("/api/realms", UriKind.Relative);

  public RealmClient(IKrakenarSettings settings) : base(settings)
  {
  }

  public virtual async Task<CreateOrReplaceRealmResult> CreateOrReplaceAsync(CreateOrReplaceRealmPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<Realm> result = id is null
      ? await PostAsync<Realm>(Path, payload, cancellationToken)
      : await PutAsync<Realm>(new Uri($"{Path}/{id}?version={version}"), payload, cancellationToken);
    return new CreateOrReplaceRealmResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<Realm?> ReadAsync(Guid? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Realm> realms = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Realm? realm = (await GetAsync<Realm>(uri, cancellationToken)).Value;
      if (realm is not null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueSlug))
    {
      Uri uri = new($"{Path}/slug:{uniqueSlug}", UriKind.Relative);
      Realm? realm = (await GetAsync<Realm>(uri, cancellationToken)).Value;
      if (realm is not null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw TooManyResultsException<Realm>.ExpectedSingle(realms.Count);
    }

    return realms.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Realm>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<Realm>(uri, payload, cancellationToken)).Value;
  }
}
