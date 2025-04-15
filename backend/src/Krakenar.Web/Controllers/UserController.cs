using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users.Commands;
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
  private readonly ICommandHandler<AuthenticateUser, User> _authenticateUser;
  private readonly ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult> _createOrReplaceUser;
  private readonly ICommandHandler<DeleteUser, User?> _deleteUser;
  private readonly IQueryHandler<ReadUser, User?> _readUser;
  private readonly ICommandHandler<ResetUserPassword, User?> _resetUserPassword;
  private readonly IQueryHandler<SearchUsers, SearchResults<User>> _searchUsers;
  private readonly ICommandHandler<SignOutUser, User?> _signOutUser;
  private readonly ICommandHandler<UpdateUser, User?> _updateUser;

  public UserController(
    ICommandHandler<AuthenticateUser, User> authenticateUser,
    ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult> createOrReplaceUser,
    ICommandHandler<DeleteUser, User?> deleteUser,
    IQueryHandler<ReadUser, User?> readUser,
    ICommandHandler<ResetUserPassword, User?> resetUserPassword,
    IQueryHandler<SearchUsers, SearchResults<User>> searchUsers,
    ICommandHandler<SignOutUser, User?> signOutUser,
    ICommandHandler<UpdateUser, User?> updateUser)
  {
    _authenticateUser = authenticateUser;
    _createOrReplaceUser = createOrReplaceUser;
    _deleteUser = deleteUser;
    _readUser = readUser;
    _resetUserPassword = resetUserPassword;
    _searchUsers = searchUsers;
    _signOutUser = signOutUser;
    _updateUser = updateUser;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<User>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    AuthenticateUser command = new(payload);
    User user = await _authenticateUser.HandleAsync(command, cancellationToken);
    return Ok(user);
  }

  [HttpPost]
  public async Task<ActionResult<User>> CreateAsync([FromBody] CreateOrReplaceUserPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceUser command = new(Id: null, payload, Version: null);
    CreateOrReplaceUserResult result = await _createOrReplaceUser.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<User>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteUser command = new(id);
    User? user = await _deleteUser.HandleAsync(command, cancellationToken);
    return user is null ? NotFound() : Ok(user);
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

  [HttpGet]
  public async Task<ActionResult<SearchResults<User>>> SearchAsync([FromQuery] SearchUsersParameters parameters, CancellationToken cancellationToken)
  {
    SearchUsersPayload payload = parameters.ToPayload();
    SearchUsers query = new(payload);
    SearchResults<User> users = await _searchUsers.HandleAsync(query, cancellationToken);
    return Ok(users);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<User>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceUser command = new(id, payload, version);
    CreateOrReplaceUserResult result = await _createOrReplaceUser.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  // TODO(fpion): remove custom identifier

  [HttpPatch("{id}/password/reset")]
  public async Task<ActionResult<User>> ResetPasswordAsync(Guid id, [FromBody] ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    ResetUserPassword command = new(id, payload);
    User? user = await _resetUserPassword.HandleAsync(command, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  // TODO(fpion): set custom identifier

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<User>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutUser command = new(id);
    User? user = await _signOutUser.HandleAsync(command, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<User>> UpdateAsync(Guid id, [FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    UpdateUser command = new(id, payload);
    User? user = await _updateUser.HandleAsync(command, cancellationToken);
    return user is null ? NotFound() : Ok(user);
  }

  private ActionResult<User> ToActionResult(CreateOrReplaceUserResult result)
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
