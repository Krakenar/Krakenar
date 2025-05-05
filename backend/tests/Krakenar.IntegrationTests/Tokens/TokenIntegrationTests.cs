using Krakenar.Contracts.Tokens;
using Krakenar.Contracts.Users;
using Krakenar.Core.Tokens;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar;
using Logitar.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Claim = System.Security.Claims.Claim;
using ClaimDto = Krakenar.Contracts.Tokens.Claim;

namespace Krakenar.Tokens;

[Trait(Traits.Category, Categories.Integration)]
public class TokenIntegrationTests : IntegrationTests
{
  private readonly ISecretManager _secretManager;
  private readonly ITokenService _tokenService;

  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  public TokenIntegrationTests() : base()
  {
    _secretManager = ServiceProvider.GetRequiredService<ISecretManager>();
    _tokenService = ServiceProvider.GetRequiredService<ITokenService>();

    _tokenHandler.InboundClaimTypeMap.Clear();
  }

  [Fact(DisplayName = "It should create the correct token.")]
  public async Task Given_Arguments_When_CreateAsync_Then_CreatedToken()
  {
    CreateTokenPayload payload = new()
    {
      IsConsumable = true,
      Algorithm = "HS512",
      Audience = "client",
      Issuer = "server",
      LifetimeSeconds = 15 * 60,
      Secret = "JpcSNtbPmpxWCBjXDMSfLnybgQuqGv8TreVsPE3U2svuzwrCa9HQ7Y48RFUdMKmA",
      Type = "custom+jwt",
      Subject = Faker.Person.UserName,
      Email = new EmailPayload(Faker.Person.Email, isVerified: true)
    };
    Claim claim = ClaimHelper.Create(Rfc7519ClaimNames.Birthdate, Faker.Person.DateOfBirth);
    payload.Claims.Add(new ClaimDto(claim.Type, claim.Value, claim.ValueType));
    CreatedToken createdToken = await _tokenService.CreateAsync(payload);

    ClaimsPrincipal principal = _tokenHandler.ValidateToken(createdToken.Token, new TokenValidationParameters
    {
      ValidAudience = payload.Audience,
      ValidIssuer = payload.Issuer,
      ValidTypes = [payload.Type],
      ValidateAudience = true,
      ValidateIssuer = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(payload.Secret))
    }, out _);

    Assert.Contains(principal.Claims, c => c.Type == Rfc7519ClaimNames.TokenId && Guid.TryParse(c.Value, out _));
    Assert.Contains(principal.Claims, c => c.Type == Rfc7519ClaimNames.Subject && c.Value == payload.Subject);
    Assert.Contains(principal.Claims, c => c.Type == Rfc7519ClaimNames.EmailAddress && c.Value == payload.Email.Address);
    Assert.Contains(principal.Claims, c => c.Type == Rfc7519ClaimNames.IsEmailVerified && bool.Parse(c.Value) == payload.Email.IsVerified);

    Claim birthdate = Assert.Single(principal.FindAll(Rfc7519ClaimNames.Birthdate));
    Assert.Equal(Faker.Person.DateOfBirth.AsUniversalTime(), ClaimHelper.ExtractDateTime(birthdate).AsUniversalTime(), TimeSpan.FromSeconds(1));

    Claim expiration = Assert.Single(principal.FindAll(Rfc7519ClaimNames.ExpirationTime));
    Assert.Equal(DateTime.UtcNow.AddSeconds(payload.LifetimeSeconds.Value), ClaimHelper.ExtractDateTime(expiration).AsUniversalTime(), TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should throw SecurityTokenBlacklistedException when the token is blacklisted.")]
  public async Task Given_BlacklistedToken_When_ValidateAsync_Then_SecurityTokenBlacklistedException()
  {
    CreateTokenPayload createPayload = new()
    {
      IsConsumable = true,
      LifetimeSeconds = 24 * 60 * 60,
      Type = "rt+jwt",
      Subject = Faker.Person.UserName,
      Email = new EmailPayload(Faker.Person.Email, isVerified: true)
    };
    createPayload.Claims.Add(new ClaimDto(Rfc7519ClaimNames.SessionId, Guid.NewGuid().ToString()));
    CreatedToken createdToken = await _tokenService.CreateAsync(createPayload);

    ValidateTokenPayload validatePayload = new(createdToken.Token)
    {
      Consume = true,
      Type = createPayload.Type
    };
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(validatePayload);
    ClaimDto tokenId = Assert.Single(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.TokenId);

    validatePayload.Consume = false;
    var exception = await Assert.ThrowsAsync<SecurityTokenBlacklistedException>(async () => await _tokenService.ValidateAsync(validatePayload));
    Assert.Contains(tokenId.Value, exception.BlacklistedIds);
  }

  [Fact(DisplayName = "It should validate the token.")]
  public async Task Given_Arguments_When_ValidateAsync_Then_ValidatedToken()
  {
    string secret = _secretManager.Resolve();

    ClaimsIdentity subject = new();
    subject.AddClaim(new Claim(Rfc7519ClaimNames.Subject, Faker.Person.UserName));
    subject.AddClaim(new Claim(Rfc7519ClaimNames.EmailAddress, Faker.Person.Email));
    subject.AddClaim(new Claim(Rfc7519ClaimNames.IsEmailVerified, bool.FalseString, ClaimValueTypes.Boolean));

    string tokenId = Guid.NewGuid().ToString();
    subject.AddClaim(new Claim(Rfc7519ClaimNames.TokenId, tokenId));

    Claim birthdateClaim = ClaimHelper.Create(Rfc7519ClaimNames.Birthdate, Faker.Person.DateOfBirth);
    subject.AddClaim(birthdateClaim);

    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = "client",
      Expires = DateTime.UtcNow.AddMinutes(15),
      Issuer = "server",
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)), "HS256"),
      Subject = subject,
      TokenType = "custom+jwt"
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    ValidateTokenPayload payload = new(token)
    {
      Consume = true,
      Audience = tokenDescriptor.Audience,
      Issuer = tokenDescriptor.Issuer,
      Secret = secret,
      Type = tokenDescriptor.TokenType
    };
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(payload);

    Assert.Equal(Faker.Person.UserName, validatedToken.Subject);
    Assert.NotNull(validatedToken.Email);
    Assert.Equal(Faker.Person.Email, validatedToken.Email.Address);
    Assert.False(validatedToken.Email.IsVerified);
    Assert.Contains(validatedToken.Claims, c => c.Name == birthdateClaim.Type && c.Value == birthdateClaim.Value && c.Type == ClaimValueTypes.Integer32);

    BlacklistedToken blacklistedToken = Assert.Single(await KrakenarContext.TokenBlacklist.AsNoTracking().Where(x => x.TokenId == tokenId).ToArrayAsync());
    Assert.NotNull(blacklistedToken.ExpiresOn);
    Assert.Equal(tokenDescriptor.Expires.Value.AddMinutes(5).AsUniversalTime(), blacklistedToken.ExpiresOn.Value, TimeSpan.FromSeconds(10));
  }
}
