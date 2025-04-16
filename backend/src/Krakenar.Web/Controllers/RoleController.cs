using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/roles")]
public class RoleController : ControllerBase
{
  protected virtual IRoleService RoleService { get; }

  public RoleController(IRoleService roleService)
  {
    RoleService = roleService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Role>> CreateAsync([FromBody] CreateOrReplaceRolePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRoleResult result = await RoleService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<Role>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Role? role = await RoleService.DeleteAsync(id, cancellationToken);
    return role is null ? NotFound() : Ok(role);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Role>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Role? role = await RoleService.ReadAsync(id, uniqueName: null, cancellationToken);
    return role is null ? NotFound() : Ok(role);
  }

  [HttpGet("name:{uniqueName}")]
  public virtual async Task<ActionResult<Role>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    Role? role = await RoleService.ReadAsync(id: null, uniqueName, cancellationToken);
    return role is null ? NotFound() : Ok(role);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<Role>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRoleResult result = await RoleService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<Role>>> SearchAsync([FromQuery] SearchRolesParameters parameters, CancellationToken cancellationToken)
  {
    SearchRolesPayload payload = parameters.ToPayload();
    SearchResults<Role> roles = await RoleService.SearchAsync(payload, cancellationToken);
    return Ok(roles);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<Role>> UpdateAsync(Guid id, [FromBody] UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    Role? role = await RoleService.UpdateAsync(id, payload, cancellationToken);
    return role is null ? NotFound() : Ok(role);
  }

  protected virtual ActionResult<Role> ToActionResult(CreateOrReplaceRoleResult result)
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
