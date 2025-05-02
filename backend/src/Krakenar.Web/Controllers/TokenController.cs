using Krakenar.Contracts.Tokens;
using Krakenar.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/tokens")]
public class TokenController : ControllerBase
{
  protected virtual ITokenService TokenService { get; }

  public TokenController(ITokenService tokenService)
  {
    TokenService = tokenService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<CreatedToken>> CreateAsync([FromBody] CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    CreatedToken createdToken = await TokenService.CreateAsync(payload, cancellationToken);
    string? url = null;
    return Created(url, createdToken);
  }

  [HttpPut]
  public virtual async Task<ActionResult<ValidatedToken>> ValidateAsync([FromBody] ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await TokenService.ValidateAsync(payload, cancellationToken);
    return Ok(validatedToken);
  }
}
