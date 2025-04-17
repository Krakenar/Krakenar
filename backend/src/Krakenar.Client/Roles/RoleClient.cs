using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Roles;

public class RoleClient : BaseClient, IRoleService
{
  protected virtual Uri Path { get; } = new("/api/roles", UriKind.Relative);

  public RoleClient(IKrakenarSettings settings) : base(settings)
  {
  }

  public virtual async Task<CreateOrReplaceRoleResult> CreateOrReplaceAsync(CreateOrReplaceRolePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<Role> result = id is null
      ? await PostAsync<Role>(Path, payload, cancellationToken)
      : await PutAsync<Role>(new Uri($"{Path}/{id}?version={version}"), payload, cancellationToken);
    return new CreateOrReplaceRoleResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<Role?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<Role>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Role?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Role> roles = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Role? role = (await GetAsync<Role>(uri, cancellationToken)).Value;
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/name:{uniqueName}", UriKind.Relative);
      Role? role = (await GetAsync<Role>(uri, cancellationToken)).Value;
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw TooManyResultsException<Role>.ExpectedSingle(roles.Count);
    }

    return roles.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Role>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<Role>(uri, payload, cancellationToken)).Value;
  }
}
