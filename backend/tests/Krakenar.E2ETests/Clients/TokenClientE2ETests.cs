using Bogus;
using Krakenar.Client.Tokens;
using Krakenar.Contracts.Tokens;
using Krakenar.Contracts.Users;
using Logitar.Security.Claims;
using Claim = Krakenar.Contracts.Tokens.Claim;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class TokenClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public TokenClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Tokens should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    TokenClient tokens = new(httpClient, KrakenarSettings);

    CreateTokenPayload createToken = new()
    {
      IsConsumable = true,
      LifetimeSeconds = 24 * 60 * 60,
      Type = "rt+jwt",
      Subject = _faker.Person.UserName,
      Email = new EmailPayload(_faker.Person.Email, isVerified: true)
    };
    createToken.Claims.Add(new Claim(Rfc7519ClaimNames.SessionId, Guid.NewGuid().ToString()));
    CreatedToken createdToken = await tokens.CreateAsync(createToken, _cancellationToken);

    ValidateTokenPayload validateToken = new(createdToken.Token)
    {
      Consume = true,
      Type = createToken.Type
    };
    ValidatedToken validatedToken = await tokens.ValidateAsync(validateToken, _cancellationToken);
    Assert.Equal(createToken.Subject, validatedToken.Subject);
    Assert.NotNull(validatedToken.Email);
    Assert.Equal(createToken.Email.Address, validatedToken.Email.Address);
    Assert.Equal(createToken.Email.IsVerified, validatedToken.Email.IsVerified);
    foreach (Claim claim in createToken.Claims)
    {
      Assert.Contains(validatedToken.Claims, c => c.Name == claim.Name && c.Value == claim.Value);
    }
  }
}
