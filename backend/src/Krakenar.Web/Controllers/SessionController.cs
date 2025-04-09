using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Core.Sessions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  private readonly IQueryHandler<ReadSession, Session?> _readSession;
  private readonly ICommandHandler<RenewSession, Session> _renewSession;
  private readonly ICommandHandler<SignInSession, Session> _signInSession;
  private readonly ICommandHandler<SignOutSession, Session?> _signOutSession;

  public SessionController(
    IQueryHandler<ReadSession, Session?> readSession,
    ICommandHandler<RenewSession, Session> renewSession,
    ICommandHandler<SignInSession, Session> signInSession,
    ICommandHandler<SignOutSession, Session?> signOutSession)
  {
    _readSession = readSession;
    _renewSession = renewSession;
    _signInSession = signInSession;
    _signOutSession = signOutSession;
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

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SignInSession command = new(payload);
    Session session = await _signInSession.HandleAsync(command, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}", UriKind.Absolute);
    return Created(location, session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutSession command = new(id);
    Session? session = await _signOutSession.HandleAsync(command, cancellationToken);
    return session is null ? NotFound() : Ok(session);
  }
}
