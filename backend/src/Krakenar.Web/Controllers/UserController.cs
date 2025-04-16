using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/users")]
public class UserController : ControllerBase
{
  protected virtual IUserService UserService { get; }

  public UserController(IUserService userService)
  {
    UserService = userService;
  }

  [HttpPatch("authenticate")]
  public virtual async Task<ActionResult<User>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    User user = await UserService.AuthenticateAsync(payload, cancellationToken);
    return Ok(user);
  }

  [HttpPost]
  public virtual async Task<ActionResult<User>> CreateAsync([FromBody] CreateOrReplaceUserPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceUserResult result = await UserService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<User>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await UserService.DeleteAsync(id, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<User>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await UserService.ReadAsync(id, uniqueName: null, customIdentifier: null, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpGet("name:{uniqueName}")]
  public virtual async Task<ActionResult<User>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    User? user = await UserService.ReadAsync(id: null, uniqueName, customIdentifier: null, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpGet("identifier/key:{key}/value:{value}")]
  public virtual async Task<ActionResult<User>> ReadAsync(string key, string value, CancellationToken cancellationToken)
  {
    User? user = await UserService.ReadAsync(id: null, uniqueName: null, new CustomIdentifierDto(key, value), cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<User>>> SearchAsync([FromQuery] SearchUsersParameters parameters, CancellationToken cancellationToken)
  {
    SearchUsersPayload payload = parameters.ToPayload();
    SearchResults<User> users = await UserService.SearchAsync(payload, cancellationToken);
    return Ok(users);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<User>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceUserResult result = await UserService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}/identifiers/key:{key}")]
  public virtual async Task<ActionResult<User>> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    User? user = await UserService.RemoveIdentifierAsync(id, key, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}/password/reset")]
  public virtual async Task<ActionResult<User>> ResetPasswordAsync(Guid id, [FromBody] ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    User? user = await UserService.ResetPasswordAsync(id, payload, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}/identifiers/key:{key}")]
  public virtual async Task<ActionResult<User>> SaveIdentifierAsync(Guid id, string key, [FromBody] SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    User? user = await UserService.SaveIdentifierAsync(id, key, payload, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}/sign/out")]
  public virtual async Task<ActionResult<User>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await UserService.SignOutAsync(id, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<User>> UpdateAsync(Guid id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    User? user = await UserService.UpdateAsync(id, payload, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  protected virtual ActionResult<User> ToActionResult(CreateOrReplaceUserResult result)
  {
    if (result.User is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/users/{result.User.Id}", UriKind.Absolute);
      return Created(location, result.User);
    }

    return Ok(result.User);
  }
}
