using Krakenar.Core.Actors;
using Krakenar.Core.Sessions;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class SessionQuerier : ISessionQuerier
{
  protected virtual IActorService ActorService { get; }

  public SessionQuerier(IActorService actorService, KrakenarContext context)
  {
    ActorService = actorService;
  }

  public virtual Task<SessionDto> ReadAsync(Session session, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<SessionDto?> ReadAsync(SessionId id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<SessionDto?> ReadAsync(Guid id, CancellationToken cancellationTokent)
  {
    throw new NotImplementedException();
  }
}
