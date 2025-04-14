using Krakenar.Contracts.Configurations;
using Krakenar.Core;
using Krakenar.Core.Configurations.Commands;
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
  private readonly ICommandHandler<ReplaceConfiguration, Configuration> _replaceConfiguration;
  private readonly ICommandHandler<UpdateConfiguration, Configuration> _updateConfiguration;

  public ConfigurationController(
    IQueryHandler<ReadConfiguration, Configuration> readConfiguration,
    ICommandHandler<ReplaceConfiguration, Configuration> replaceConfiguration,
    ICommandHandler<UpdateConfiguration, Configuration> updateConfiguration)
  {
    _readConfiguration = readConfiguration;
    _replaceConfiguration = replaceConfiguration;
    _updateConfiguration = updateConfiguration;
  }

  [HttpGet]
  public async Task<ActionResult<Configuration>> ReadAsync(CancellationToken cancellationToken)
  {
    ReadConfiguration query = new();
    Configuration configuration = await _readConfiguration.HandleAsync(query, cancellationToken);
    return Ok(configuration);
  }

  [HttpPut]
  public async Task<ActionResult<Configuration>> ReplaceAsync([FromBody] ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    ReplaceConfiguration command = new(payload, version);
    Configuration configuration = await _replaceConfiguration.HandleAsync(command, cancellationToken);
    return Ok(configuration);

  }

  [HttpPatch]
  public async Task<ActionResult<Configuration>> UpdateAsync([FromBody] UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    UpdateConfiguration command = new(payload);
    Configuration configuration = await _updateConfiguration.HandleAsync(command, cancellationToken);
    return Ok(configuration);
  }
}
