using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;

namespace Krakenar.Client.Users;

public interface IUserClient : IUserService
{
  Task<User> AuthenticateAsync(AuthenticateUserPayload payload, RequestContext? context);
  Task<CreateOrReplaceUserResult> CreateOrReplaceAsync(CreateOrReplaceUserPayload payload, Guid? id, long? version, RequestContext? context);
  Task<User?> DeleteAsync(Guid id, RequestContext? context);
  Task<User?> RemoveIdentifierAsync(Guid id, string key, RequestContext? context);
  Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, RequestContext? context);
  Task<User?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifier? customIdentifier, RequestContext? context);
  Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, RequestContext? context);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, RequestContext? context);
  Task<User?> SignOutAsync(Guid id, RequestContext? context);
  Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, RequestContext? context);
}
