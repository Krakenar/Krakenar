using Krakenar.Contracts.Sessions;
using Krakenar.Models.Account;
using Krakenar.Web;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public AccountController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpPost("api/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInAccountPayload input, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(input.Username, input.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    HttpContext.SignIn(session);

    CurrentUser currentUser = new(session);
    return Ok(currentUser);
  }
}
