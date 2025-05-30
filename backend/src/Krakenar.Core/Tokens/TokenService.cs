using Krakenar.Contracts.Tokens;
using Krakenar.Core.Tokens.Commands;

namespace Krakenar.Core.Tokens;

public class TokenService : ITokenService
{
  protected virtual ICommandBus CommandBus { get; }

  public TokenService(ICommandBus commandBus)
  {
    CommandBus = commandBus;
  }

  public virtual async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    CreateToken command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ValidatedToken> ValidateAsync(ValidateTokenPayload payload, CancellationToken cancellationToken)
  {
    ValidateToken command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
