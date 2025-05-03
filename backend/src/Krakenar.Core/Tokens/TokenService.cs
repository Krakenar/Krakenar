using Krakenar.Contracts.Tokens;
using Krakenar.Core.Tokens.Commands;

namespace Krakenar.Core.Tokens;

public class TokenService : ITokenService
{
  protected virtual ICommandHandler<CreateToken, CreatedToken> CreateToken { get; }
  protected virtual ICommandHandler<ValidateToken, ValidatedToken> ValidateToken { get; }

  public TokenService(ICommandHandler<CreateToken, CreatedToken> createToken, ICommandHandler<ValidateToken, ValidatedToken> validateToken)
  {
    CreateToken = createToken;
    ValidateToken = validateToken;
  }

  public virtual async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    CreateToken command = new(payload);
    return await CreateToken.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    ValidateToken command = new(payload);
    return await ValidateToken.HandleAsync(command, cancellationToken);
  }
}
