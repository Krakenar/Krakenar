using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Models.Account;
using Krakenar.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json; // TODO(fpion): refactor

namespace Krakenar.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly bool _exposeErrorDetail;
  private readonly ILogger<AccountController> _logger;
  private readonly ISessionService _sessionService;

  public AccountController(IConfiguration configuration, ILogger<AccountController> logger, ISessionService sessionService)
  {
    string? exposeErrorDetailValue = Environment.GetEnvironmentVariable("EXPOSE_ERROR_DETAIL");
    _exposeErrorDetail = !string.IsNullOrWhiteSpace(exposeErrorDetailValue) && bool.TryParse(exposeErrorDetailValue, out bool exposeErrorDetail)
      ? exposeErrorDetail : configuration.GetValue<bool>("ExposeErrorDetail");

    _logger = logger;
    _sessionService = sessionService;
  }

  [HttpPost("api/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInAccountPayload input, CancellationToken cancellationToken)
  {
    try
    {
      SignInSessionPayload payload = new(input.Username, input.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
      Session session = await _sessionService.SignInAsync(payload, cancellationToken);
      HttpContext.SignIn(session);

      CurrentUser currentUser = new(session);
      return Ok(currentUser);
    }
    catch (InvalidCredentialsException exception)
    {
      if (_exposeErrorDetail)
      {
        throw;
      }

      string serializedError = JsonSerializer.Serialize(exception.Error);
      _logger.LogError(exception, "Invalid credentials: {Error}", serializedError);

      Error error = new("InvalidCredentials", "The specified credentials did not match.");
      return Problem(
        detail: error.Message,
        instance: Request.GetDisplayUrl(),
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid Credentials",
        type: null,
        extensions: new Dictionary<string, object?> { ["error"] = error });
    }
  }
}
