using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly IQueryHandler<ReadUser, User?> _readUser;

  public UserController(
    IQueryHandler<ReadUser, User?> readUser)
  {
    _readUser = readUser;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadUser query = new(id, UniqueName: null, CustomIdentifier: null);
    User? user = await _readUser.HandleAsync(query, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<User>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadUser query = new(Id: null, uniqueName, CustomIdentifier: null);
    User? user = await _readUser.HandleAsync(query, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpGet("identifier/key:{key}/value:{value}")]
  public async Task<ActionResult<User>> ReadAsync(string key, string value, CancellationToken cancellationToken)
  {
    ReadUser query = new(Id: null, UniqueName: null, new CustomIdentifierDto(key, value));
    User? user = await _readUser.HandleAsync(query, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }
}
