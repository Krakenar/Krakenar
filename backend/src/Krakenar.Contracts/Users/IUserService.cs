using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Users;

public interface IUserService
{
  Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken = default);
  Task<CreateOrReplaceUserResult> CreateOrReplaceAsync(CreateOrReplaceUserPayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken = default);
  Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid? id = null, string? uniqueName = null, CustomIdentifier? customIdentifier = null, CancellationToken cancellationToken = default);
  Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
}
