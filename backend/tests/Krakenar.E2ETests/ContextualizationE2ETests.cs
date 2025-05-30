using Bogus;
using Krakenar.Client;
using Krakenar.Client.Users;
using Krakenar.Contracts.Users;

namespace Krakenar;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ContextualizationE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public ContextualizationE2ETests() : base()
  {
  }

  [Fact(DisplayName = "It should contextualize API requests.")]
  public async Task Given_RequestContext_When_ApiCall_Then_RequestContextualized()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient client = new();
    UserClient users = new(client, KrakenarSettings);

    RequestContext context = new(_cancellationToken)
    {
      Basic = KrakenarSettings.Basic,
      Realm = Realm.UniqueSlug
    };

    string uniqueName = _faker.Internet.UserName();
    User? user = await users.ReadAsync(id: null, uniqueName, customIdentifier: null, context);
    if (user is null)
    {
      CreateOrReplaceUserPayload userPayload = new(uniqueName);
      CreateOrReplaceUserResult userResult = await users.CreateOrReplaceAsync(userPayload, id: null, version: null, context);
      Assert.True(userResult.Created);
      Assert.NotNull(userResult.User);
      user = userResult.User;
    }
    Assert.True(user.Email is null || !user.Email.IsVerified);
    Guid userId = user.Id;
    context.User = userId.ToString();

    string emailAddress = _faker.Internet.Email();
    UpdateUserPayload payload = new()
    {
      Email = new Contracts.Change<EmailPayload>(new EmailPayload(emailAddress, isVerified: true))
    };
    user = await users.UpdateAsync(user.Id, payload, context);
    Assert.NotNull(user);
    Assert.Equal(userId, user.Id);

    Assert.Equal(Realm.Id, user.UpdatedBy.RealmId);
    Assert.Equal(user.Id, user.UpdatedBy.Id);
    Assert.Equal(emailAddress, user.UpdatedBy.EmailAddress);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.NotNull(user.Email);
    Assert.Equal(emailAddress, user.Email.Address);
    Assert.True(user.Email.IsVerified);

    Assert.NotNull(user.Email.VerifiedBy);
    Assert.Equal(Realm.Id, user.Email.VerifiedBy.RealmId);
    Assert.Equal(user.Id, user.Email.VerifiedBy.Id);
    Assert.Equal(emailAddress, user.Email.VerifiedBy.EmailAddress);

    Assert.NotNull(user.Email.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Email.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    user = await users.DeleteAsync(user.Id, context);
    Assert.NotNull(user);
    Assert.Equal(userId, user.Id);
  }
}
