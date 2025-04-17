using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;

namespace Krakenar.Client.Sessions;

public class SessionClient : BaseClient, ISessionService
{
  protected virtual Uri Path { get; } = new("/api/sessions", UriKind.Relative);

  public SessionClient(IKrakenarSettings settings) : base(settings)
  {
  }

  public virtual async Task<Session> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    return (await PostAsync<Session>(Path, payload, cancellationToken)).Value ?? throw new NotImplementedException(); // TODO(fpion): implement;
  }

  public virtual async Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<Session>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/renew", UriKind.Relative);
    return (await PutAsync<Session>(uri, payload, cancellationToken)).Value ?? throw new NotImplementedException(); // TODO(fpion): implement;;
  }

  public virtual async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["user"] = [payload.UserId];
    parameters["active"] = [payload.IsActive];
    parameters["persistent"] = [payload.IsPersistent];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Session>>(uri, cancellationToken)).Value ?? throw new NotImplementedException(); // TODO(fpion): implement
  }

  public virtual async Task<Session> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/sign/in", UriKind.Relative);
    return (await PostAsync<Session>(uri, payload, cancellationToken)).Value ?? throw new NotImplementedException(); // TODO(fpion): implement;
  }

  public virtual async Task<Session?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}/sign/out", UriKind.Relative);
    return (await PatchAsync<Session>(uri, cancellationToken)).Value;
  }
}
