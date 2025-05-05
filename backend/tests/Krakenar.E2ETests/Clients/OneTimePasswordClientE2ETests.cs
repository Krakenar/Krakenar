using Bogus;
using Krakenar.Client.Passwords;
using Krakenar.Client.Users;
using Krakenar.Contracts;
using Krakenar.Contracts.Passwords;
using Krakenar.Contracts.Users;
using Logitar;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class OneTimePasswordClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public OneTimePasswordClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "One-Time Passwords (OTP) should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    KrakenarSettings.Realm = Realm.UniqueSlug;
    using HttpClient oneTimePasswordClient = new();
    OneTimePasswordClient oneTimePasswords = new(oneTimePasswordClient, KrakenarSettings);
    using HttpClient userClient = new();
    UserClient users = new(userClient, KrakenarSettings);

    Guid userId = Guid.Parse("1fc234db-0f36-4536-a29e-354157db5fa3");
    CreateOrReplaceUserPayload createOrReplaceUser = new(_faker.Person.UserName)
    {
      Email = new EmailPayload(_faker.Person.Email, isVerified: true)
    };
    CreateOrReplaceUserResult userResult = await users.CreateOrReplaceAsync(createOrReplaceUser, userId, version: null, _cancellationToken);
    User? user = userResult.User;
    Assert.NotNull(user);
    Assert.NotNull(user.Email);

    CreateOneTimePasswordPayload createOneTimePassword = new("1234567890", 6)
    {
      Id = Guid.NewGuid(),
      User = $"  {user.Email.Address.ToUpperInvariant()}  ",
      ExpiresOn = DateTime.Now.AddMinutes(15),
      MaximumAttempts = 5
    };
    createOneTimePassword.CustomAttributes.Add(new CustomAttribute("IpAddress", _faker.Internet.Ip()));
    OneTimePassword oneTimePassword = await oneTimePasswords.CreateAsync(createOneTimePassword, _cancellationToken);
    Assert.NotNull(oneTimePassword);
    Assert.Equal(createOneTimePassword.Id, oneTimePassword.Id);
    Assert.Equal(user, oneTimePassword.User);
    Assert.NotNull(oneTimePassword.Password);
    Assert.True(oneTimePassword.Password.All(createOneTimePassword.Characters.Contains));
    Assert.Equal(createOneTimePassword.Length, oneTimePassword.Password.Length);
    Assert.NotNull(oneTimePassword.ExpiresOn);
    Assert.Equal(createOneTimePassword.ExpiresOn.Value.AsUniversalTime(), oneTimePassword.ExpiresOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(createOneTimePassword.MaximumAttempts, oneTimePassword.MaximumAttempts);
    Assert.Equal(0, oneTimePassword.AttemptCount);
    Assert.Null(oneTimePassword.ValidationSucceededOn);
    Assert.Equal(createOneTimePassword.CustomAttributes, oneTimePassword.CustomAttributes);

    ValidateOneTimePasswordPayload validateOneTimePassword = new(oneTimePassword.Password);
    validateOneTimePassword.CustomAttributes.Add(new CustomAttribute("UserAgent", _faker.Internet.UserAgent()));
    oneTimePassword = (await oneTimePasswords.ValidateAsync(oneTimePassword.Id, validateOneTimePassword, _cancellationToken))!;
    Assert.NotNull(oneTimePassword);
    Assert.Equal(1, oneTimePassword.AttemptCount);
    Assert.NotNull(oneTimePassword.ValidationSucceededOn);
    Assert.Equal(DateTime.UtcNow, oneTimePassword.ValidationSucceededOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(2, oneTimePassword.CustomAttributes.Count);
    Assert.Contains(Assert.Single(createOneTimePassword.CustomAttributes), oneTimePassword.CustomAttributes);
    Assert.Contains(Assert.Single(validateOneTimePassword.CustomAttributes), oneTimePassword.CustomAttributes);

    oneTimePassword = (await oneTimePasswords.ReadAsync(oneTimePassword.Id, _cancellationToken))!;
    Assert.NotNull(oneTimePassword);
    Assert.NotNull(oneTimePassword.ValidationSucceededOn);
  }
}
