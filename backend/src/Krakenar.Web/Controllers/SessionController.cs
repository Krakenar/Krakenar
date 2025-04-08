using Krakenar.Contracts.Sessions;
using Krakenar.Core;
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

  public SessionController(IQueryHandler<ReadSession, Session?> readSession)
  {
    _readSession = readSession;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSession query = new(id);
    Session? session = await _readSession.HandleAsync(query, cancellationToken);
    return session is null ? NotFound() : Ok(session);
  }
}
