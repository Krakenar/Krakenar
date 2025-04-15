using Krakenar.Contracts.Configurations;
using Krakenar.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
  protected virtual IConfigurationService ConfigurationService { get; }

  public ConfigurationController(IConfigurationService configurationService)
  {
    ConfigurationService = configurationService;
  }

  [HttpGet]
  public virtual async Task<ActionResult<Configuration>> ReadAsync(CancellationToken cancellationToken)
  {
    Configuration configuration = await ConfigurationService.ReadAsync(cancellationToken);
    return Ok(configuration);
  }

  [HttpPut]
  public virtual async Task<ActionResult<Configuration>> ReplaceAsync([FromBody] ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    Configuration configuration = await ConfigurationService.ReplaceAsync(payload, version, cancellationToken);
    return Ok(configuration);

  }

  [HttpPatch]
  public virtual async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    Configuration configuration = await ConfigurationService.UpdateAsync(payload, cancellationToken);
    return Ok(configuration);
  }
}
