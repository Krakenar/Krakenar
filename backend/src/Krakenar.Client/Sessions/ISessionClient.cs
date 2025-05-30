using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;

namespace Krakenar.Client.Sessions;

public interface ISessionClient : ISessionService
{
  Task<Session> CreateAsync(CreateSessionPayload payload, RequestContext? context);
  Task<Session?> ReadAsync(Guid id, RequestContext? context);
  Task<Session> RenewAsync(RenewSessionPayload payload, RequestContext? context);
  Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, RequestContext? context);
  Task<Session> SignInAsync(SignInSessionPayload payload, RequestContext? context);
  Task<Session?> SignOutAsync(Guid id, RequestContext? context);
}
