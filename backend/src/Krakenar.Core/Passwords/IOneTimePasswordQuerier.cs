using Krakenar.Core.Users;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords;

public interface IOneTimePasswordQuerier
{
  Task<IReadOnlyCollection<OneTimePasswordId>> FindIdsAsync(UserId userId, CancellationToken cancellationToken = default);

  Task<OneTimePasswordDto> ReadAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);
  Task<OneTimePasswordDto?> ReadAsync(OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePasswordDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
