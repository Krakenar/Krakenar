using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Core.Users.Commands;
using Krakenar.Core.Users.Queries;
using Logitar.CQRS;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users;

public class UserService : IUserService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public UserService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<UserDto> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    AuthenticateUser command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<CreateOrReplaceUserResult> CreateOrReplaceAsync(CreateOrReplaceUserPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceUser command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteUser command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    RemoveUserIdentifier command = new(id, key);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifierDto? customIdentifier, CancellationToken cancellationToken)
  {
    ReadUser query = new(id, uniqueName, customIdentifier);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<UserDto?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    ResetUserPassword command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    SaveUserIdentifier command = new(id, key, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SearchResults<UserDto>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    SearchUsers query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<UserDto?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SignOutUser command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<UserDto?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    UpdateUser command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
