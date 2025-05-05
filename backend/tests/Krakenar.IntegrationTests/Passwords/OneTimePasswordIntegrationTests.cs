using Krakenar.Contracts;
using Krakenar.Contracts.Passwords;
using Krakenar.Core;
using Krakenar.Core.Passwords;
using Krakenar.Core.Users;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using OneTimePassword = Krakenar.Core.Passwords.OneTimePassword;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Passwords;

[Trait(Traits.Category, Categories.Integration)]
public class OneTimePasswordIntegrationTests : IntegrationTests
{
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserRepository _userRepository;

  private User _user = null!;
  private OneTimePassword _oneTimePassword = null!;
  private string _password = null!;

  public OneTimePasswordIntegrationTests() : base()
  {
    _oneTimePasswordRepository = ServiceProvider.GetRequiredService<IOneTimePasswordRepository>();
    _oneTimePasswordService = ServiceProvider.GetRequiredService<IOneTimePasswordService>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _user = new User(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), password: null, ActorId, UserId.NewId(Realm.Id));
    await _userRepository.SaveAsync(_user);

    Password password = _passwordManager.Generate("0123456789ABCDEF", length: 8, out string passwordString);
    _oneTimePassword = new OneTimePassword(password, expiresOn: DateTime.Now.AddMinutes(10), maximumAttempts: 5, _user, ActorId, OneTimePasswordId.NewId(Realm.Id));
    _password = passwordString;
    await _oneTimePasswordRepository.SaveAsync(_oneTimePassword);
  }

  [Theory(DisplayName = "It should create a new One-Time Password (OTP).")]
  [InlineData(null, false)]
  [InlineData("4d5f4639-2592-4a18-a59d-a387c935ae98", true)]
  public async Task Given_NotExisting_When_CreateAsync_Then_Created(string? idValue, bool withUser)
  {
    Guid? id = idValue is null ? null : Guid.Parse(idValue);

    CreateOneTimePasswordPayload payload = new("1234567890", length: 6)
    {
      Id = id,
      User = withUser ? _user.UniqueName.Value : null,
      ExpiresOn = DateTime.Now.AddHours(1),
      MaximumAttempts = 5
    };
    payload.CustomAttributes.Add(new CustomAttribute("Purpose", "MFA"));

    OneTimePasswordDto? oneTimePassword = await _oneTimePasswordService.CreateAsync(payload);
    Assert.NotNull(oneTimePassword);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, oneTimePassword.Id);
    }
    else
    {
      Assert.NotEqual(default, oneTimePassword.Id);
    }
    Assert.Equal(2, oneTimePassword.Version);
    Assert.Equal(Actor, oneTimePassword.CreatedBy);
    Assert.Equal(DateTime.UtcNow, oneTimePassword.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, oneTimePassword.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, oneTimePassword.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.NotNull(oneTimePassword.Realm);
    Assert.Equal(RealmDto, oneTimePassword.Realm);
    if (withUser)
    {
      Assert.NotNull(oneTimePassword.User);
      Assert.Equal(_user.EntityId, oneTimePassword.User.Id);
    }
    else
    {
      Assert.Null(oneTimePassword.User);
    }

    Assert.NotNull(oneTimePassword.Password);
    Assert.True(oneTimePassword.Password.All(payload.Characters.Contains));
    Assert.Equal(payload.Length, oneTimePassword.Password.Length);

    Assert.NotNull(oneTimePassword.ExpiresOn);
    Assert.Equal(payload.ExpiresOn.Value.AsUniversalTime(), oneTimePassword.ExpiresOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(payload.MaximumAttempts, oneTimePassword.MaximumAttempts);

    Assert.Equal(0, oneTimePassword.AttemptCount);
    Assert.Null(oneTimePassword.ValidationSucceededOn);

    Assert.Equal(payload.CustomAttributes, oneTimePassword.CustomAttributes);
  }

  [Fact(DisplayName = "It should read the One-Time Password (OTP) by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    OneTimePasswordDto? oneTimePassword = await _oneTimePasswordService.ReadAsync(_oneTimePassword.EntityId);
    Assert.NotNull(oneTimePassword);
    Assert.Equal(_oneTimePassword.EntityId, oneTimePassword.Id);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password (OTP) cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _oneTimePasswordService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when creating a One-Time Password (OTP) with an ID which is already used.")]
  public async Task Given_IdAlreadyUsed_When_CreateAsync_Then_IdAlreadyUsedException()
  {
    CreateOneTimePasswordPayload payload = new("1234567890", length: 6)
    {
      Id = _oneTimePassword.EntityId
    };

    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException<OneTimePassword>>(async () => await _oneTimePasswordService.CreateAsync(payload));
    Assert.Equal(Realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal("OneTimePassword", exception.EntityType);
    Assert.Equal(payload.Id, exception.EntityId);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should validate an existing One-Time Password (OTP).")]
  public async Task Given_Existing_When_ValidateAsync_Then_Validated()
  {
    ValidateOneTimePasswordPayload payload = new(_password);
    payload.CustomAttributes.Add(new CustomAttribute("IpAddress", Faker.Internet.Ip()));

    OneTimePasswordDto? oneTimePassword = await _oneTimePasswordService.ValidateAsync(_oneTimePassword.EntityId, payload);
    Assert.NotNull(oneTimePassword);

    Assert.Equal(_oneTimePassword.EntityId, oneTimePassword.Id);
    Assert.Equal(_oneTimePassword.Version + 2, oneTimePassword.Version);
    Assert.Equal(Actor, oneTimePassword.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, oneTimePassword.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.NotNull(oneTimePassword.Realm);
    Assert.Equal(RealmDto, oneTimePassword.Realm);
    Assert.NotNull(oneTimePassword.User);
    Assert.Equal(_user.EntityId, oneTimePassword.User.Id);

    Assert.Null(oneTimePassword.Password);

    Assert.NotNull(oneTimePassword.ExpiresOn);
    Assert.NotNull(oneTimePassword.MaximumAttempts);

    Assert.Equal(1, oneTimePassword.AttemptCount);
    Assert.NotNull(oneTimePassword.ValidationSucceededOn);
    Assert.Equal(DateTime.UtcNow, oneTimePassword.ValidationSucceededOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.CustomAttributes, oneTimePassword.CustomAttributes);
  }
}
