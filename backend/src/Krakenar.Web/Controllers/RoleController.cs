using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
using Krakenar.Web.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/roles")]
public class RoleController : ControllerBase
{
  private readonly ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult> _createOrReplaceRole;
  private readonly ICommandHandler<DeleteRole, Role?> _deleteRole;
  private readonly IQueryHandler<ReadRole, Role?> _readRole;
  private readonly IQueryHandler<SearchRoles, SearchResults<Role>> _searchRoles;
  private readonly ICommandHandler<UpdateRole, Role?> _updateRole;

  public RoleController(
    ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult> createOrReplaceRole,
    ICommandHandler<DeleteRole, Role?> deleteRole,
    IQueryHandler<ReadRole, Role?> readRole,
    IQueryHandler<SearchRoles, SearchResults<Role>> searchRoles,
    ICommandHandler<UpdateRole, Role?> updateRole)
  {
    _createOrReplaceRole = createOrReplaceRole;
    _deleteRole = deleteRole;
    _readRole = readRole;
    _searchRoles = searchRoles;
    _updateRole = updateRole;
  }

  [HttpPost]
  public async Task<ActionResult<Role>> CreateAsync([FromBody] CreateOrReplaceRolePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRole command = new(Id: null, payload, Version: null);
    CreateOrReplaceRoleResult result = await _createOrReplaceRole.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Role>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteRole command = new(id);
    Role? role = await _deleteRole.HandleAsync(command, cancellationToken);
    return role is null ? NotFound() : Ok(role);
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

  [HttpGet]
  public async Task<ActionResult<SearchResults<Role>>> SearchAsync([FromQuery] SearchRolesParameters parameters, CancellationToken cancellationToken)
  {
    SearchRolesPayload payload = parameters.ToPayload();
    SearchRoles query = new(payload);
    SearchResults<Role> roles = await _searchRoles.HandleAsync(query, cancellationToken);
    return Ok(roles);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Role>> UpdateAsync(Guid id, [FromBody] UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    UpdateRole command = new(id, payload);
    Role? role = await _updateRole.HandleAsync(command, cancellationToken);
    return role is null ? NotFound() : Ok(role);
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
