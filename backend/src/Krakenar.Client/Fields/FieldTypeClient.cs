using Krakenar.Contracts;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Fields;

public class FieldTypeClient : BaseClient, IFieldTypeService
{
  protected virtual Uri Path { get; } = new("/api/fields/types", UriKind.Relative);

  public FieldTypeClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<CreateOrReplaceFieldTypeResult> CreateOrReplaceAsync(CreateOrReplaceFieldTypePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<FieldType> result = id is null
      ? await PostAsync<FieldType>(Path, payload, cancellationToken)
      : await PutAsync<FieldType>(new Uri($"{Path}/{id}?version={version}", UriKind.Relative), payload, cancellationToken);
    return new CreateOrReplaceFieldTypeResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<FieldType?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<FieldType>(uri, cancellationToken)).Value;
  }

  public virtual async Task<FieldType?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    Dictionary<Guid, FieldType> roles = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      FieldType? role = (await GetAsync<FieldType>(uri, cancellationToken)).Value;
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/name:{uniqueName}", UriKind.Relative);
      FieldType? role = (await GetAsync<FieldType>(uri, cancellationToken)).Value;
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw TooManyResultsException<FieldType>.ExpectedSingle(roles.Count);
    }

    return roles.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<FieldType>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["type"] = [payload.DataType];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<FieldType>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<FieldType?> UpdateAsync(Guid id, UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<FieldType>(uri, payload, cancellationToken)).Value;
  }
}
