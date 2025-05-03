namespace Krakenar.Core.Tokens;

public interface ITokenManager
{
  Task<string> CreateAsync(ClaimsIdentity subject, string secret, CancellationToken cancellationToken = default);
  Task<string> CreateAsync(ClaimsIdentity subject, string secret, CreateTokenOptions? options, CancellationToken cancellationToken = default);

  Task<ClaimsPrincipal> ValidateAsync(string token, string secret, CancellationToken cancellationToken = default);
  Task<ClaimsPrincipal> ValidateAsync(string token, string secret, ValidateTokenOptions? options, CancellationToken cancellationToken = default);
}
