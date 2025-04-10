﻿using Krakenar.Contracts.Roles;
using Krakenar.Core;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/roles")]
public class RoleController : ControllerBase
{
  private readonly ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult> _createOrReplaceRole;
  private readonly IQueryHandler<ReadRole, Role?> _readRole;

  public RoleController(
    ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult> createOrReplaceRole,
    IQueryHandler<ReadRole, Role?> readRole)
  {
    _createOrReplaceRole = createOrReplaceRole;
    _readRole = readRole;
  }

  [HttpPost]
  public async Task<ActionResult<Role>> CreateAsync([FromBody] CreateOrReplaceRolePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRole command = new(Id: null, payload, Version: null);
    CreateOrReplaceRoleResult result = await _createOrReplaceRole.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Role>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRole query = new(id, UniqueName: null);
    Role? role = await _readRole.HandleAsync(query, cancellationToken);
    return role is null ? NotFound() : Ok(role);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<Role>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadRole query = new(Id: null, uniqueName);
    Role? role = await _readRole.HandleAsync(query, cancellationToken);
    return role is null ? NotFound() : Ok(role);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Role>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRole command = new(id, payload, version);
    CreateOrReplaceRoleResult result = await _createOrReplaceRole.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  private ActionResult<Role> ToActionResult(CreateOrReplaceRoleResult result)
  {
    if (result.Role is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/roles/{result.Role.Id}", UriKind.Absolute);
      return Created(location, result.Role);
    }

    return Ok(result.Role);
  }
}
