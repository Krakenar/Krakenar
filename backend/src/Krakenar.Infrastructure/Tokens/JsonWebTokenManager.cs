using Krakenar.Core.Tokens;
using Logitar;
using Logitar.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Krakenar.Infrastructure.Tokens;

public class JsonWebTokenManager : ITokenManager
{
  protected virtual ITokenBlacklist TokenBlacklist { get; }
  protected virtual JwtSecurityTokenHandler TokenHandler { get; }

  public JsonWebTokenManager(ITokenBlacklist tokenBlacklist) : this(tokenBlacklist, new())
  {
  }

  public JsonWebTokenManager(ITokenBlacklist tokenBlacklist, JwtSecurityTokenHandler tokenHandler)
  {
    TokenBlacklist = tokenBlacklist;
    TokenHandler = tokenHandler;
    TokenHandler.InboundClaimTypeMap.Clear();
  }

  public virtual async Task<string> CreateAsync(ClaimsIdentity subject, string secret, CancellationToken cancellationToken)
  {
    return await CreateAsync(subject, secret, options: null, cancellationToken);
  }
  public virtual Task<string> CreateAsync(ClaimsIdentity subject, string secret, CreateTokenOptions? options, CancellationToken cancellationToken)
  {
    options ??= new();

    SigningCredentials signingCredentials = new(GetSecurityKey(secret), options.SigningAlgorithm);

    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = options.Audience,
      Expires = options.Expires?.AsUniversalTime(),
      IssuedAt = options.IssuedAt?.AsUniversalTime(),
      Issuer = options.Issuer,
      NotBefore = options.NotBefore?.AsUniversalTime(),
      SigningCredentials = signingCredentials,
      Subject = subject,
      TokenType = options.Type
    };

    SecurityToken securityToken = TokenHandler.CreateToken(tokenDescriptor);
    string tokenString = TokenHandler.WriteToken(securityToken);
    return Task.FromResult(tokenString);
  }

  public virtual async Task<ClaimsPrincipal> ValidateAsync(string token, string secret, CancellationToken cancellationToken)
  {
    return await ValidateAsync(token, secret, options: null, cancellationToken);
  }
  public virtual async Task<ClaimsPrincipal> ValidateAsync(string token, string secret, ValidateTokenOptions? options, CancellationToken cancellationToken)
  {
    options ??= new();

    TokenValidationParameters validationParameters = new()
    {
      IssuerSigningKey = GetSecurityKey(secret),
      ValidAudiences = options.ValidAudiences,
      ValidIssuers = options.ValidIssuers,
      ValidateAudience = options.ValidAudiences.Count > 0,
      ValidateIssuer = options.ValidIssuers.Count > 0,
      ValidateIssuerSigningKey = true
    };
    if (options.ValidTypes.Count > 0)
    {
      validationParameters.ValidTypes = options.ValidTypes;
    }

    ClaimsPrincipal claimsPrincipal = TokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

    HashSet<string> tokenIds = claimsPrincipal.FindAll(Rfc7519ClaimNames.TokenId).Select(claim => claim.Value).ToHashSet();
    if (tokenIds.Count > 0)
    {
      IReadOnlyCollection<string> blacklistedIds = await TokenBlacklist.GetBlacklistedAsync(tokenIds, cancellationToken);
      if (blacklistedIds.Count > 0)
      {
        throw new SecurityTokenBlacklistedException(blacklistedIds);
      }

      if (options.Consume)
      {
        Claim? expiresClaim = claimsPrincipal.FindAll(Rfc7519ClaimNames.ExpirationTime).OrderBy(x => x.Value).FirstOrDefault();
        DateTime? expiresOn = expiresClaim is null ? null : ClaimHelper.ExtractDateTime(expiresClaim).Add(validationParameters.ClockSkew);

        await TokenBlacklist.BlacklistAsync(tokenIds, expiresOn, cancellationToken);
      }
    }

    return claimsPrincipal;
  }

  protected virtual SymmetricSecurityKey GetSecurityKey(string secret) => new(Encoding.ASCII.GetBytes(secret));
}
