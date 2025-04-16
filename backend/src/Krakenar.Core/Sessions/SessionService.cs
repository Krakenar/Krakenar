using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Core.Sessions.Queries;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions;

public class SessionService : ISessionService
{
  protected virtual ICommandHandler<CreateSession, SessionDto> CreateSession { get; }
  protected virtual IQueryHandler<ReadSession, SessionDto?> ReadSession { get; }
  protected virtual ICommandHandler<RenewSession, SessionDto> RenewSession { get; }
  protected virtual IQueryHandler<SearchSessions, SearchResults<SessionDto>> SearchSessions { get; }
  protected virtual ICommandHandler<SignInSession, SessionDto> SignInSession { get; }
  protected virtual ICommandHandler<SignOutSession, SessionDto?> SignOutSession { get; }

  public SessionService(
    ICommandHandler<CreateSession, SessionDto> createSession,
    IQueryHandler<ReadSession, SessionDto?> readSession,
    IQueryHandler<SearchSessions, SearchResults<SessionDto>> searchSessions,
    ICommandHandler<RenewSession, SessionDto> renewSession,
    ICommandHandler<SignInSession, SessionDto> signInSession,
    ICommandHandler<SignOutSession, SessionDto?> signOutSession)
  {
    CreateSession = createSession;
    ReadSession = readSession;
    RenewSession = renewSession;
    SearchSessions = searchSessions;
    SignInSession = signInSession;
    SignOutSession = signOutSession;
  }

  public virtual async Task<SessionDto> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    CreateSession command = new(payload);
    return await CreateSession.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SessionDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSession query = new(id);
    return await ReadSession.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SessionDto> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    RenewSession command = new(payload);
    return await RenewSession.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SearchResults<SessionDto>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    SearchSessions query = new(payload);
    return await SearchSessions.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SessionDto> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SignInSession command = new(payload);
    return await SignInSession.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SessionDto?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutSession command = new(id);
    return await SignOutSession.HandleAsync(command, cancellationToken);
  }
}
