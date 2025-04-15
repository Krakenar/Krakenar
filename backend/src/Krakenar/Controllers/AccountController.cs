using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Models.Account;
using Krakenar.Web;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly ICommandHandler<SignInSession, Session> _signIn;

  public AccountController(ICommandHandler<SignInSession, Session> signIn)
  {
    _signIn = signIn;
  }

  [HttpPost("api/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInAccountPayload input, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(input.Username, input.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
    SignInSession command = new(payload);
    Session session = await _signIn.HandleAsync(command, cancellationToken);
    HttpContext.SignIn(session);

    CurrentUser currentUser = new(session);
    return Ok(currentUser);
  }
}
