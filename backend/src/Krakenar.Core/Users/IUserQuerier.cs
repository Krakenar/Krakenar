using Krakenar.Contracts.Users;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users;

public interface IUserQuerier
{
  Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);
  Task<UserId?> FindIdAsync(Identifier key, CustomIdentifier value, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<UserId>> FindIdsAsync(IEmail email, CancellationToken cancellationToken = default);

  Task<UserDto> ReadAsync(User user, CancellationToken cancellationToken = default);
  Task<UserDto?> ReadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<UserDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<UserDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<UserDto?> ReadAsync(CustomIdentifierDto identifier, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<UserDto>> ReadAsync(IEmail email, CancellationToken cancellationToken = default);
}
