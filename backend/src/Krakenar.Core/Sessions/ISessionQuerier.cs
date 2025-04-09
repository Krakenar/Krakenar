using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions;

public interface ISessionQuerier
{
  Task<SessionDto> ReadAsync(Session session, CancellationToken cancellationToken = default);
  Task<SessionDto?> ReadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<SessionDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
