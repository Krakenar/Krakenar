using FluentValidation;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Tokens;
using Krakenar.Contracts.Users;
using Krakenar.Core.Tokens.Validators;
using Logitar;
using Logitar.Security.Claims;
using ClaimDto = Krakenar.Contracts.Tokens.Claim;

namespace Krakenar.Core.Tokens.Commands;

/// <exception cref="ValidationException"></exception>
public record CreateToken(CreateTokenPayload Payload) : ICommand<CreatedToken>, ISensitiveActivity
{
  public IActivity Anonymize()
  {
    if (string.IsNullOrWhiteSpace(Payload.Secret))
    {
      return this;
    }

    CreateToken clone = this.DeepClone();
    clone.Payload.Secret = Payload.Secret.Mask();
    return clone;
  }
}

internal class CreateTokenCommandHandler : ICommandHandler<CreateToken, CreatedToken>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISecretManager SecretManager { get; }
  protected virtual ITokenManager TokenManager { get; }

  public CreateTokenCommandHandler(IApplicationContext applicationContext, ISecretManager secretManager, ITokenManager tokenManager)
  {
    ApplicationContext = applicationContext;
    SecretManager = secretManager;
    TokenManager = tokenManager;
  }

  public virtual async Task<CreatedToken> HandleAsync(CreateToken command, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = command.Payload;
    new CreateTokenValidator().ValidateAndThrow(payload);

    Realm? realm = ApplicationContext.Realm;
    string baseUrl = ApplicationContext.BaseUrl;

    ClaimsIdentity subject = CreateSubject(payload);
    string secret = SecretManager.Resolve(payload.Secret);
    CreateTokenOptions options = new()
    {
      Audience = TokenHelper.ResolveAudience(payload.Audience, realm, baseUrl),
      Issuer = TokenHelper.ResolveIssuer(payload.Issuer, realm, baseUrl)
    };
    if (!string.IsNullOrWhiteSpace(payload.Algorithm))
    {
      options.SigningAlgorithm = payload.Algorithm.Trim();
    }
    if (!string.IsNullOrWhiteSpace(payload.Type))
    {
      options.Type = payload.Type.Trim();
    }
    if (payload.LifetimeSeconds.HasValue)
    {
      options.Expires = DateTime.UtcNow.AddSeconds(payload.LifetimeSeconds.Value);
    }
    string createdToken = await TokenManager.CreateAsync(subject, secret, options, cancellationToken);
    return new CreatedToken(createdToken);
  }

  protected virtual ClaimsIdentity CreateSubject(CreateTokenPayload payload)
  {
    ClaimsIdentity subject = new();

    if (payload.IsConsumable)
    {
      subject.AddClaim(new(Rfc7519ClaimNames.TokenId, Guid.NewGuid().ToString()));
    }

    if (!string.IsNullOrWhiteSpace(payload.Subject))
    {
      subject.AddClaim(new(Rfc7519ClaimNames.Subject, payload.Subject.Trim()));
    }

    if (payload.Email is not null)
    {
      Email email = new(payload.Email);
      subject.AddClaim(new(Rfc7519ClaimNames.EmailAddress, email.Address));
      subject.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, email.IsVerified.ToString()));
    }

    foreach (ClaimDto claim in payload.Claims)
    {
      subject.AddClaim(new(claim.Name, claim.Value, claim.Type));
    }

    return subject;
  }
}
