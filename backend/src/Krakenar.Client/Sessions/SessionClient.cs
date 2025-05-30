using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;

namespace Krakenar.Client.Sessions;

public class SessionClient : BaseClient, ISessionClient
{
  protected virtual Uri Path { get; } = new("/api/sessions", UriKind.Relative);

  public SessionClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<Session> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await CreateAsync(payload, context);
  }
  public virtual async Task<Session> CreateAsync(CreateSessionPayload payload, RequestContext? context)
  {
    return (await PostAsync<Session>(Path, payload, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, Path, payload);
  }

  public virtual async Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await ReadAsync(id, context);
  }
  public virtual async Task<Session?> ReadAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<Session>(uri, context)).Value;
  }

  public virtual async Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await RenewAsync(payload, context);

  }
  public virtual async Task<Session> RenewAsync(RenewSessionPayload payload, RequestContext? context)
  {
    Uri uri = new($"{Path}/renew", UriKind.Relative);
    return (await PutAsync<Session>(uri, payload, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(RenewAsync), HttpMethod.Put, uri, payload);
  }

  public virtual async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SearchAsync(payload, context);
  }
  public virtual async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, RequestContext? context)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["user"] = [payload.UserId];
    parameters["active"] = [payload.IsActive];
    parameters["persistent"] = [payload.IsPersistent];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Session>>(uri, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Session> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SignInAsync(payload, context);
  }
  public virtual async Task<Session> SignInAsync(SignInSessionPayload payload, RequestContext? context)
  {
    Uri uri = new($"{Path}/sign/in", UriKind.Relative);
    return (await PostAsync<Session>(uri, payload, context)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SignInAsync), HttpMethod.Post, uri, payload);
  }

  public virtual async Task<Session?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await SignOutAsync(id, context);
  }
  public virtual async Task<Session?> SignOutAsync(Guid id, RequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/sign/out", UriKind.Relative);
    return (await PatchAsync<Session>(uri, context)).Value;
  }
}
