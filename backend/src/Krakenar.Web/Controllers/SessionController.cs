using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Core.Sessions.Queries;
using Krakenar.Web.Models.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  private readonly ICommandHandler<CreateSession, Session> _createSession;
  private readonly IQueryHandler<ReadSession, Session?> _readSession;
  private readonly ICommandHandler<RenewSession, Session> _renewSession;
  private readonly IQueryHandler<SearchSessions, SearchResults<Session>> _searchSessions;
  private readonly ICommandHandler<SignInSession, Session> _signInSession;
  private readonly ICommandHandler<SignOutSession, Session?> _signOutSession;

  public SessionController(
    ICommandHandler<CreateSession, Session> createSession,
    IQueryHandler<ReadSession, Session?> readSession,
    ICommandHandler<RenewSession, Session> renewSession,
    IQueryHandler<SearchSessions, SearchResults<Session>> searchSessions,
    ICommandHandler<SignInSession, Session> signInSession,
    ICommandHandler<SignOutSession, Session?> signOutSession)
  {
    _createSession = createSession;
    _readSession = readSession;
    _renewSession = renewSession;
    _searchSessions = searchSessions;
    _signInSession = signInSession;
    _signOutSession = signOutSession;
  }

  [HttpPost]
  public async Task<ActionResult<Session>> CreateAsync([FromBody] CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    CreateSession command = new(payload);
    Session session = await _createSession.HandleAsync(command, cancellationToken);
    return Created(session);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSession query = new(id);
    Session? session = await _readSession.HandleAsync(query, cancellationToken);
    return session is null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public async Task<ActionResult<Session>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    RenewSession command = new(payload);
    Session session = await _renewSession.HandleAsync(command, cancellationToken);
    return Ok(session);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Session>>> SearchAsync([FromQuery] SearchSessionsParameters parameters, CancellationToken cancellationToken)
  {
    SearchSessionsPayload payload = parameters.ToPayload();
    SearchSessions query = new(payload);
    SearchResults<Session> sessions = await _searchSessions.HandleAsync(query, cancellationToken);
    return Ok(sessions);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SignInSession command = new(payload);
    Session session = await _signInSession.HandleAsync(command, cancellationToken);
    return Created(session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutSession command = new(id);
    Session? session = await _signOutSession.HandleAsync(command, cancellationToken);
    return session is null ? NotFound() : Ok(session);
  }

  private ActionResult<Session> Created(Session session)
  {
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}", UriKind.Absolute);
    return Created(location, session);
  }
}
