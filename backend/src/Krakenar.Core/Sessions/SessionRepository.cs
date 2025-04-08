using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions;

public interface ISessionRepository
{
  Task<Session?> LoadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<Session?> LoadAsync(SessionId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Session session, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Session> sessions, CancellationToken cancellationToken = default);
}

public class SessionRepository : Repository, ISessionRepository
{
  public SessionRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Session?> LoadAsync(SessionId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<Session?> LoadAsync(SessionId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Session>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Session session, CancellationToken cancellationToken)
  {
    await base.SaveAsync(session, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Session> sessions, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sessions, cancellationToken);
  }
}
