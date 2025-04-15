using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Users;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions;

public interface ISessionQuerier
{
  Task<IReadOnlyCollection<SessionId>> FindActiveIdsAsync(UserId userId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<SessionId>> FindIdsAsync(UserId userId, CancellationToken cancellationToken = default);

  Task<SessionDto> ReadAsync(Session session, CancellationToken cancellationToken = default);
  Task<SessionDto?> ReadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<SessionDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<SessionDto>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken = default);
}
