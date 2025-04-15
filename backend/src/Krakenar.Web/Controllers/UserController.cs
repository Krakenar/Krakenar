using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users.Queries;
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
  private readonly IQueryHandler<ReadUser, User?> _readUser;
  private readonly IQueryHandler<SearchUsers, SearchResults<User>> _searchUsers;

  public UserController(
    IQueryHandler<ReadUser, User?> readUser,
    IQueryHandler<SearchUsers, SearchResults<User>> searchUsers)
  {
    _readUser = readUser;
    _searchUsers = searchUsers;
  }

  // TODO(fpion): authenticate

  // TODO(fpion): create

  // TODO(fpion): delete

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

  [HttpGet]
  public async Task<ActionResult<SearchResults<User>>> SearchAsync([FromQuery] SearchUsersParameters parameters, CancellationToken cancellationToken)
  {
    SearchUsersPayload payload = parameters.ToPayload();
    SearchUsers query = new(payload);
    SearchResults<User> users = await _searchUsers.HandleAsync(query, cancellationToken);
    return Ok(users);
  }

  // TODO(fpion): replace

  // TODO(fpion): remove custom identifier

  // TODO(fpion): reset password

  // TODO(fpion): set custom identifier

  // TODO(fpion): sign-out

  // TODO(fpion): update
}
