using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;

namespace Krakenar.Client.Users;

public class UserClient : BaseClient, IUserClient
{
  protected virtual Uri Path { get; } = new("/api/users", UriKind.Relative);

  public UserClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await AuthenticateAsync(payload, context);
  }
  public virtual async Task<User> AuthenticateAsync(AuthenticateUserPayload payload, RequestContext? context)
  {
    Uri uri = new($"{Path}/authenticate", UriKind.Relative);
    return (await PatchAsync<User>(uri, payload, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(AuthenticateAsync), HttpMethod.Patch, uri, payload);
  }

  public virtual async Task<CreateOrReplaceUserResult> CreateOrReplaceAsync(CreateOrReplaceUserPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await CreateOrReplaceAsync(payload, id, version, context);
  }
  public virtual async Task<CreateOrReplaceUserResult> CreateOrReplaceAsync(CreateOrReplaceUserPayload payload, Guid? id, long? version, RequestContext? context)
  {
    ApiResult<User> result = id is null
      ? await PostAsync<User>(Path, payload, context)
      : await PutAsync<User>(new Uri($"{Path}/{id}?version={version}", UriKind.Relative), payload, context);
    return new CreateOrReplaceUserResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await DeleteAsync(id, context);
  }
  public virtual async Task<User?> DeleteAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<User>(uri, context)).Value;
  }

  public virtual async Task<User?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifier? customIdentifier, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await ReadAsync(id, uniqueName, customIdentifier, context);
  }
  public virtual async Task<User?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifier? customIdentifier, RequestContext? context)
  {
    Dictionary<Guid, User> users = new(capacity: 3);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      User? user = (await GetAsync<User>(uri, context)).Value;
      if (user is not null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/name:{uniqueName}", UriKind.Relative);
      User? user = (await GetAsync<User>(uri, context)).Value;
      if (user is not null)
      {
        users[user.Id] = user;
      }
    }

    if (customIdentifier is not null)
    {
      Uri uri = new($"{Path}/identifier/key:{customIdentifier.Key}/value:{customIdentifier.Value}", UriKind.Relative);
      User? user = (await GetAsync<User>(uri, context)).Value;
      if (user is not null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw TooManyResultsException<User>.ExpectedSingle(users.Count);
    }

    return users.SingleOrDefault().Value;
  }

  public virtual async Task<User?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await RemoveIdentifierAsync(id, key, context);
  }
  public virtual async Task<User?> RemoveIdentifierAsync(Guid id, string key, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/identifiers/key:{key}", UriKind.Relative);
    return (await DeleteAsync<User>(uri, context)).Value;
  }

  public virtual async Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await ResetPasswordAsync(id, payload, context);
  }
  public virtual async Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/password/reset", UriKind.Relative);
    return (await PatchAsync<User>(uri, payload, context)).Value;
  }

  public virtual async Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SaveIdentifierAsync(id, key, payload, context);
  }
  public virtual async Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/identifiers/key:{key}", UriKind.Relative);
    return (await PutAsync<User>(uri, payload, context)).Value;
  }

  public virtual async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SearchAsync(payload, context);
  }
  public virtual async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, RequestContext? context)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["password"] = [payload.HasPassword];
    parameters["disabled"] = [payload.IsDisabled];
    parameters["confirmed"] = [payload.IsConfirmed];
    parameters["authenticated"] = [payload.HasAuthenticated];
    parameters["role"] = [payload.RoleId];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<User>>(uri, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SignOutAsync(id, context);
  }
  public virtual async Task<User?> SignOutAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/sign/out", UriKind.Relative);
    return (await PatchAsync<User>(uri, context)).Value;
  }

  public virtual async Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await UpdateAsync(id, payload, context);
  }
  public virtual async Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<User>(uri, payload, context)).Value;
  }
}
