using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Core.Users.Commands;
using Krakenar.Core.Users.Queries;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users;

public class UserService : IUserService
{
  protected virtual ICommandHandler<AuthenticateUser, UserDto> AuthenticateUser { get; }
  protected virtual ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult> CreateOrReplaceUser { get; }
  protected virtual ICommandHandler<DeleteUser, UserDto?> DeleteUser { get; }
  protected virtual IQueryHandler<ReadUser, UserDto?> ReadUser { get; }
  protected virtual ICommandHandler<RemoveUserIdentifier, UserDto?> RemoveUserIdentifier { get; }
  protected virtual ICommandHandler<ResetUserPassword, UserDto?> ResetUserPassword { get; }
  protected virtual ICommandHandler<SaveUserIdentifier, UserDto?> SaveUserIdentifier { get; }
  protected virtual IQueryHandler<SearchUsers, SearchResults<UserDto>> SearchUsers { get; }
  protected virtual ICommandHandler<SignOutUser, UserDto?> SignOutUser { get; }
  protected virtual ICommandHandler<UpdateUser, UserDto?> UpdateUser { get; }

  public UserService(
    ICommandHandler<AuthenticateUser, UserDto> authenticateUser,
    ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult> createOrReplaceUser,
    ICommandHandler<DeleteUser, UserDto?> deleteUser,
    IQueryHandler<ReadUser, UserDto?> readUser,
    ICommandHandler<RemoveUserIdentifier, UserDto?> removeUserIdentifier,
    ICommandHandler<ResetUserPassword, UserDto?> resetUserPassword,
    ICommandHandler<SaveUserIdentifier, UserDto?> saveUserIdentifier,
    IQueryHandler<SearchUsers, SearchResults<UserDto>> searchUsers,
    ICommandHandler<SignOutUser, UserDto?> signOutUser,
    ICommandHandler<UpdateUser, UserDto?> updateUser)
  {
    AuthenticateUser = authenticateUser;
    CreateOrReplaceUser = createOrReplaceUser;
    DeleteUser = deleteUser;
    ReadUser = readUser;
    RemoveUserIdentifier = removeUserIdentifier;
    ResetUserPassword = resetUserPassword;
    SaveUserIdentifier = saveUserIdentifier;
    SearchUsers = searchUsers;
    SignOutUser = signOutUser;
    UpdateUser = updateUser;
  }

  public virtual async Task<UserDto> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    AuthenticateUser command = new(payload);
    return await AuthenticateUser.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<CreateOrReplaceUserResult> CreateOrReplaceAsync(CreateOrReplaceUserPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceUser command = new(id, payload, version);
    return await CreateOrReplaceUser.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteUser command = new(id);
    return await DeleteUser.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    RemoveUserIdentifier command = new(id, key);
    return await RemoveUserIdentifier.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifierDto? customIdentifier, CancellationToken cancellationToken)
  {
    ReadUser query = new(id, uniqueName, customIdentifier);
    return await ReadUser.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<UserDto?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    ResetUserPassword command = new(id, payload);
    return await ResetUserPassword.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    SaveUserIdentifier command = new(id, key, payload);
    return await SaveUserIdentifier.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SearchResults<UserDto>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    SearchUsers query = new(payload);
    return await SearchUsers.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<UserDto?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutUser command = new(id);
    return await SignOutUser.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    UpdateUser command = new(id, payload);
    return await UpdateUser.HandleAsync(command, cancellationToken);
  }
}
