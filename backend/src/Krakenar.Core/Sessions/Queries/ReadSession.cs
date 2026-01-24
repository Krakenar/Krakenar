using Logitar.CQRS;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Queries;

public record ReadSession(Guid Id) : IQuery<SessionDto?>;

public class ReadSessionHandler : IQueryHandler<ReadSession, SessionDto?>
{
  protected virtual ISessionQuerier SessionQuerier { get; }

  public ReadSessionHandler(ISessionQuerier sessionQuerier)
  {
    SessionQuerier = sessionQuerier;
  }

  public virtual async Task<SessionDto?> HandleAsync(ReadSession query, CancellationToken cancellationToken)
  {
    return await SessionQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
