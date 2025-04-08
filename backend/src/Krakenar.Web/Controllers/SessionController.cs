﻿using Krakenar.Contracts.Sessions;
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
  private readonly ICommandHandler<SignInSession, Session> _signInSession;

  public SessionController(
    IQueryHandler<ReadSession, Session?> readSession,
    ICommandHandler<SignInSession, Session> signInSession)
  {
    _readSession = readSession;
    _signInSession = signInSession;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSession query = new(id);
    Session? session = await _readSession.HandleAsync(query, cancellationToken);
    return session is null ? NotFound() : Ok(session);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SignInSession command = new(payload);
    Session session = await _signInSession.HandleAsync(command, cancellationToken);
    return Ok(session);
  }
}
