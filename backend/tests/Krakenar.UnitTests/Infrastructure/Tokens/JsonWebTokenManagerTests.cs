using Bogus;
using Krakenar.Core.Tokens;
using Logitar;
using Logitar.Security.Claims;
using Logitar.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace Krakenar.Infrastructure.Tokens;

[Trait(Traits.Category, Categories.Unit)]
public class JsonWebTokenManagerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ITokenBlacklist> _tokenBlacklist = new();
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  private readonly JsonWebTokenManager _manager;

  public JsonWebTokenManagerTests()
  {
    _manager = new(_tokenBlacklist.Object, _tokenHandler);
  }

  [Fact(DisplayName = "CreateAsync: it should create a token for an identity without options.")]
  public async Task Given_NoOption_When_CreateAsync_Then_Created()
  {
    ClaimsIdentity subject = new();
    subject.AddClaim(new(Rfc7519ClaimNames.Subject, Guid.NewGuid().ToString()));
    subject.AddClaim(new(Rfc7519ClaimNames.Username, _faker.Person.UserName));
    subject.AddClaim(new(Rfc7519ClaimNames.FullName, _faker.Person.FullName));
    subject.AddClaim(new(Rfc7519ClaimNames.EmailAddress, _faker.Person.Email));
    subject.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, bool.TrueString.ToLower(), ClaimValueTypes.Boolean));
    subject.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, DateTime.UtcNow));
    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);

    string token = await _manager.CreateAsync(subject, secret, _cancellationToken);

    TokenValidationParameters parameters = new()
    {
      IssuerSigningKey = GetSecurityKey(secret),
      ValidateAudience = false,
      ValidateIssuer = false,
      ValidateIssuerSigningKey = true
    };
    ClaimsPrincipal principal = _tokenHandler.ValidateToken(token, parameters, out _);
    foreach (Claim claim in subject.Claims)
    {
      if (claim.Type == Rfc7519ClaimNames.AuthenticationTime)
      {
        Assert.Contains(principal.Claims, c => c.Type == claim.Type && c.Value == claim.Value && c.ValueType == "http://www.w3.org/2001/XMLSchema#integer32");
      }
      else
      {
        Assert.Contains(principal.Claims, c => c.Type == claim.Type && c.Value == claim.Value && c.ValueType == claim.ValueType);
      }
    }
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.IssuedAt && (DateTime.UtcNow - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(10));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.NotBefore && (DateTime.UtcNow - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(10));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.ExpirationTime && (DateTime.UtcNow.AddHours(1) - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "CreateAsync: it should create a token for an identity with options.")]
  public async Task Given_Options_When_CreateAsync_Then_Created()
  {
    string sub = Guid.NewGuid().ToString();
    ClaimsIdentity subject = new([new Claim(Rfc7519ClaimNames.Subject, sub)]);
    string secret = RandomStringGenerator.GetString(Secret.MaximumLength);
    CreateTokenOptions options = new()
    {
      Type = "rt+jwt",
      SigningAlgorithm = "HS512",
      Audience = "tests",
      Issuer = "kraken",
      Expires = DateTime.Now.AddDays(7),
      IssuedAt = DateTime.Now.AddMinutes(5),
      NotBefore = DateTime.Now.AddHours(1)
    };

    string token = await _manager.CreateAsync(subject, secret, options, _cancellationToken);

    TokenValidationParameters parameters = new()
    {
      IssuerSigningKey = GetSecurityKey(secret),
      ValidAudience = options.Audience,
      ValidIssuer = options.Issuer,
      ValidTypes = [options.Type],
      ValidateIssuerSigningKey = true,
      ValidateLifetime = false
    };
    ClaimsPrincipal principal = _tokenHandler.ValidateToken(token, parameters, out _);
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.Subject && claim.Value == sub);
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.IssuedAt && (options.IssuedAt.Value.AsUniversalTime() - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(10));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.NotBefore && (options.NotBefore.Value.AsUniversalTime() - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(10));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.ExpirationTime && (options.Expires.Value.AsUniversalTime() - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "ValidateAsync: it should blacklist consumed token IDs.")]
  public async Task Given_Consume_When_ValidateAsync_Then_TokenIdBlacklisted()
  {
    string secret = RandomStringGenerator.GetString(Secret.MaximumLength);
    string tokenId = Guid.NewGuid().ToString();
    ClaimsIdentity subject = new([new Claim(Rfc7519ClaimNames.TokenId, tokenId)]);
    SecurityTokenDescriptor descriptor = new()
    {
      Audience = "tests",
      Issuer = "kraken",
      SigningCredentials = new SigningCredentials(GetSecurityKey(secret), "HS512"),
      Subject = subject,
      TokenType = "rt+jwt"
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(descriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    _tokenBlacklist.Setup(x => x.GetBlacklistedAsync(It.Is<IEnumerable<string>>(y => y.Single() == tokenId), _cancellationToken)).ReturnsAsync([]);

    ValidateTokenOptions options = new()
    {
      Consume = true
    };
    options.ValidTypes.Add(descriptor.TokenType);
    options.ValidAudiences.Add(descriptor.Audience);
    options.ValidIssuers.Add(descriptor.Issuer);
    ClaimsPrincipal principal = await _manager.ValidateAsync(token, secret, options, _cancellationToken);

    _tokenBlacklist.Verify(x => x.GetBlacklistedAsync(It.Is<IEnumerable<string>>(y => y.Single() == tokenId), _cancellationToken), Times.Once());
    _tokenBlacklist.Verify(x => x.BlacklistAsync(
      It.Is<IEnumerable<string>>(y => y.Single() == tokenId),
      It.Is<DateTime?>(expires => expires.HasValue && (DateTime.UtcNow - expires.Value) < TimeSpan.FromSeconds(1)),
      _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "ValidateAsync: it should check the token is not blacklisted when it has an ID.")]
  public async Task Given_TokenId_When_ValidateAsync_Then_CheckNotBlacklisted()
  {
    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    string tokenId = Guid.NewGuid().ToString();
    ClaimsIdentity subject = new([new Claim(Rfc7519ClaimNames.TokenId, tokenId)]);
    SecurityTokenDescriptor descriptor = new()
    {
      SigningCredentials = new SigningCredentials(GetSecurityKey(secret), "HS256"),
      Subject = subject
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(descriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    _tokenBlacklist.Setup(x => x.GetBlacklistedAsync(It.Is<IEnumerable<string>>(y => y.Single() == tokenId), _cancellationToken)).ReturnsAsync([]);

    ClaimsPrincipal principal = await _manager.ValidateAsync(token, secret, _cancellationToken);

    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.TokenId && claim.Value == tokenId);

    _tokenBlacklist.Verify(x => x.GetBlacklistedAsync(It.Is<IEnumerable<string>>(y => y.Single() == tokenId), _cancellationToken), Times.Once());
    _tokenBlacklist.Verify(x => x.BlacklistAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "ValidateAsync: it should throw SecurityTokenBlacklistedException when the token is blacklisted.")]
  public async Task Given_BlacklistedId_When_ValidateAsync_Then_SecurityTokenBlacklistedException()
  {
    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    string tokenId = Guid.NewGuid().ToString();
    ClaimsIdentity subject = new([new Claim(Rfc7519ClaimNames.TokenId, tokenId)]);
    SecurityTokenDescriptor descriptor = new()
    {
      SigningCredentials = new SigningCredentials(GetSecurityKey(secret), "HS256"),
      Subject = subject
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(descriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    string[] tokenIds = [tokenId, Guid.Empty.ToString()];
    _tokenBlacklist.Setup(x => x.GetBlacklistedAsync(It.Is<IEnumerable<string>>(y => y.Single() == tokenId), _cancellationToken)).ReturnsAsync(tokenIds);

    var exception = await Assert.ThrowsAsync<SecurityTokenBlacklistedException>(async () => await _manager.ValidateAsync(token, secret, _cancellationToken));
    Assert.Equal(tokenIds, exception.BlacklistedIds);
  }

  [Fact(DisplayName = "ValidateAsync: it should validate a token without options.")]
  public async Task Given_NoOption_When_ValidateAsync_Then_Validated()
  {
    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    ClaimsIdentity subject = new([new Claim(Rfc7519ClaimNames.Username, _faker.Person.UserName)]);
    SecurityTokenDescriptor descriptor = new()
    {
      SigningCredentials = new SigningCredentials(GetSecurityKey(secret), "HS256"),
      Subject = subject
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(descriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    ClaimsPrincipal principal = await _manager.ValidateAsync(token, secret, _cancellationToken);

    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.Username && claim.Value == _faker.Person.UserName);
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.IssuedAt && (DateTime.UtcNow - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(1));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.NotBefore && (DateTime.UtcNow - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(1));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.ExpirationTime && (DateTime.UtcNow.AddHours(1) - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(1));

    _tokenBlacklist.Verify(x => x.GetBlacklistedAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never());
    _tokenBlacklist.Verify(x => x.BlacklistAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  [Fact(DisplayName = "ValidateAsync: it should validate a token with options.")]
  public async Task Given_Options_When_ValidateAsync_Then_Validated()
  {
    string secret = RandomStringGenerator.GetString(Secret.MaximumLength);
    ClaimsIdentity subject = new([new Claim(Rfc7519ClaimNames.EmailAddress, _faker.Person.Email)]);
    SecurityTokenDescriptor descriptor = new()
    {
      Audience = "tests",
      Issuer = "kraken",
      SigningCredentials = new SigningCredentials(GetSecurityKey(secret), "HS512"),
      Subject = subject,
      TokenType = "rt+jwt"
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(descriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    ValidateTokenOptions options = new()
    {
      Consume = true
    };
    options.ValidTypes.Add(descriptor.TokenType);
    options.ValidAudiences.Add(descriptor.Audience);
    options.ValidIssuers.Add(descriptor.Issuer);
    ClaimsPrincipal principal = await _manager.ValidateAsync(token, secret, options, _cancellationToken);

    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.EmailAddress && claim.Value == _faker.Person.Email);
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.IssuedAt && (DateTime.UtcNow - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(1));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.NotBefore && (DateTime.UtcNow - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(1));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.ExpirationTime && (DateTime.UtcNow.AddHours(1) - ClaimHelper.ExtractDateTime(claim)) < TimeSpan.FromSeconds(1));

    _tokenBlacklist.Verify(x => x.GetBlacklistedAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never());
    _tokenBlacklist.Verify(x => x.BlacklistAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Never());
  }

  private static SymmetricSecurityKey GetSecurityKey(string secret) => new(Encoding.ASCII.GetBytes(secret));
}
