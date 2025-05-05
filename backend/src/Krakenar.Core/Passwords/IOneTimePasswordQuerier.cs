using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords;

public interface IOneTimePasswordQuerier
{
  Task<OneTimePasswordDto> ReadAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);
  Task<OneTimePasswordDto?> ReadAsync(OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePasswordDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
