using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Core.Sessions.Queries;
using Logitar.CQRS;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions;

public class SessionService : ISessionService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public SessionService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<SessionDto> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    CreateSession command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SessionDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSession query = new(id);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SessionDto> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    RenewSession command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SearchResults<SessionDto>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    SearchSessions query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SessionDto> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SignInSession command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SessionDto?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutSession command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
