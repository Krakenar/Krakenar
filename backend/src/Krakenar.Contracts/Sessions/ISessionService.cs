using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Sessions;

public interface ISessionService
{
  Task<Session> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken = default);
  Task<Session?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
}
