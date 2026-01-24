using FluentValidation;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Tokens;
using Krakenar.Contracts.Users;
using Krakenar.Core.Logging;
using Krakenar.Core.Tokens.Validators;
using Logitar;
using Logitar.CQRS;
using Logitar.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace Krakenar.Core.Tokens.Commands;

/// <exception cref="SecurityTokenBlacklistedException"></exception>
/// <exception cref="ValidationException"></exception>
public record ValidateToken(ValidateTokenPayload Payload) : IAnonymizable, ICommand<ValidatedToken>
{
  public object? Anonymize()
  {
    if (Payload.Consume && string.IsNullOrWhiteSpace(Payload.Secret))
    {
      return this;
    }

    ValidateToken clone = this.DeepClone();
    if (!Payload.Consume)
    {
      clone.Payload.Token = Payload.Token.Mask();
    }
    if (!string.IsNullOrWhiteSpace(Payload.Secret))
    {
      clone.Payload.Secret = Payload.Secret.Mask();
    }
    return clone;
  }
}

internal class ValidateTokenCommandHandler : ICommandHandler<ValidateToken, ValidatedToken>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISecretManager _secretManager;
  private readonly ITokenManager _tokenManager;

  public ValidateTokenCommandHandler(IApplicationContext applicationContext, ISecretManager secretManager, ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _secretManager = secretManager;
    _tokenManager = tokenManager;
  }

  public virtual async Task<ValidatedToken> HandleAsync(ValidateToken command, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = command.Payload;
    new ValidateTokenValidator().ValidateAndThrow(payload);

    Realm? realm = _applicationContext.Realm;
    string baseUrl = _applicationContext.BaseUrl;

    string secret = _secretManager.Resolve(payload.Secret);
    ValidateTokenOptions options = new()
    {
      ValidAudiences = [TokenHelper.ResolveAudience(payload.Audience, realm, baseUrl)],
      ValidIssuers = [TokenHelper.ResolveIssuer(payload.Issuer, realm, baseUrl)],
      Consume = payload.Consume
    };
    if (!string.IsNullOrWhiteSpace(payload.Type))
    {
      options.ValidTypes.Add(payload.Type.Trim());
    }
    ClaimsPrincipal principal = await _tokenManager.ValidateAsync(payload.Token, secret, options, cancellationToken);
    return CreateResult(principal);
  }

  protected virtual ValidatedToken CreateResult(ClaimsPrincipal principal)
  {
    ValidatedToken result = new();

    string? emailAddress = null;
    bool? isEmailVerified = null;
    foreach (Claim claim in principal.Claims)
    {

      switch (claim.Type)
      {
        case Rfc7519ClaimNames.EmailAddress:
          emailAddress = claim.Value;
          break;
        case Rfc7519ClaimNames.IsEmailVerified:
          isEmailVerified = bool.Parse(claim.Value);
          break;
        case Rfc7519ClaimNames.Subject:
          result.Subject = claim.Value;
          break;
        default:
          result.Claims.Add(new(claim.Type, claim.Value, claim.ValueType));
          break;
      }
    }
    if (emailAddress is not null)
    {
      result.Email = new Email(emailAddress)
      {
        IsVerified = isEmailVerified ?? false
      };
    }
    else if (isEmailVerified.HasValue)
    {
      result.Claims.Add(new(Rfc7519ClaimNames.IsEmailVerified, isEmailVerified.Value.ToString()));
    }

    return result;
  }
}
