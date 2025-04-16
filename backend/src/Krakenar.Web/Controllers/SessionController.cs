using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  protected virtual ISessionService SessionService { get; }

  public SessionController(ISessionService sessionService)
  {
    SessionService = sessionService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Session>> CreateAsync([FromBody] CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    Session session = await SessionService.CreateAsync(payload, cancellationToken);
    return Created(session);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Session>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await SessionService.ReadAsync(id, cancellationToken);
    return session is null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public virtual async Task<ActionResult<Session>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    Session session = await SessionService.RenewAsync(payload, cancellationToken);
    return Ok(session);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<Session>>> SearchAsync([FromQuery] SearchSessionsParameters parameters, CancellationToken cancellationToken)
  {
    SearchSessionsPayload payload = parameters.ToPayload();
    SearchResults<Session> sessions = await SessionService.SearchAsync(payload, cancellationToken);
    return Ok(sessions);
  }

  [HttpPost("sign/in")]
  public virtual async Task<ActionResult<Session>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    Session session = await SessionService.SignInAsync(payload, cancellationToken);
    return Created(session);
  }

  [HttpPatch("{id}/sign/out")]
  public virtual async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await SessionService.SignOutAsync(id, cancellationToken);
    return session is null ? NotFound() : Ok(session);
  }

  protected virtual ActionResult<Session> Created(Session session)
  {
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}", UriKind.Absolute);
    return Created(location, session);
  }
}
