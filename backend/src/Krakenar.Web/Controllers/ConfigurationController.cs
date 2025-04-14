using Krakenar.Contracts.Configurations;
using Krakenar.Core;
using Krakenar.Core.Configurations.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/configuration")]
public class ConfigurationController : ControllerBase
{
  private readonly IQueryHandler<ReadConfiguration, Configuration> _readConfiguration;

  public ConfigurationController(IQueryHandler<ReadConfiguration, Configuration> readConfiguration)
  {
    _readConfiguration = readConfiguration;
  }

  [HttpGet]
  public async Task<ActionResult<Configuration>> ReadAsync(CancellationToken cancellationToken)
  {
    ReadConfiguration query = new();
    Configuration configuration = await _readConfiguration.HandleAsync(query, cancellationToken);
    return Ok(configuration);
  }

  // TODO(fpion): replace

  // TODO(fpion): update
}
