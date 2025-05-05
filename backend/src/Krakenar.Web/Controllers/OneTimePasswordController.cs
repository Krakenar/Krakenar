using Krakenar.Contracts.Passwords;
using Krakenar.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/one-time-passwords")]
public class OneTimePasswordController : ControllerBase
{
  protected virtual IOneTimePasswordService OneTimePasswordService { get; }

  public OneTimePasswordController(IOneTimePasswordService oneTimePasswordService)
  {
    OneTimePasswordService = oneTimePasswordService;
  }

  [HttpPost]
  public async Task<ActionResult<OneTimePasswordDto>> CreateAsync([FromBody] CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    OneTimePasswordDto oneTimePassword = await OneTimePasswordService.CreateAsync(payload, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/one-time-passwords/{oneTimePassword.Id}", UriKind.Absolute);
    return Created(location, oneTimePassword);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<OneTimePasswordDto>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    OneTimePasswordDto? oneTimePassword = await OneTimePasswordService.ReadAsync(id, cancellationToken);
    return oneTimePassword is null ? NotFound() : Ok(oneTimePassword);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<OneTimePasswordDto>> ValidateAsync(Guid id, [FromBody] ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    OneTimePasswordDto? oneTimePassword = await OneTimePasswordService.ValidateAsync(id, payload, cancellationToken);
    return oneTimePassword is null ? NotFound() : Ok(oneTimePassword);
  }
}
