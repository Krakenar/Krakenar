using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Moq;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOneTimePasswordHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IOneTimePasswordQuerier> _oneTimePasswordQuerier = new();
  private readonly Mock<IOneTimePasswordRepository> _oneTimePasswordRepository = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<IUserManager> _userManager = new();

  private readonly CreateOneTimePasswordHandler _handler;

  public CreateOneTimePasswordHandlerTests()
  {
    _handler = new(_applicationContext.Object, _oneTimePasswordQuerier.Object, _oneTimePasswordRepository.Object, _passwordManager.Object, _userManager.Object);
  }

  [Theory(DisplayName = "It should create a new One-Time Password (OTP).")]
  [InlineData(null, false)]
  [InlineData("076595ab-6b12-4ffa-bd8d-845e7d513a63", true)]
  public async Task Given_Valid_When_HandleAsync_Then_OneTimePasswordCreated(string? idValue, bool withUser)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    CreateOneTimePasswordPayload payload = new()
    {
      Id = id,
      Characters = "0123456789",
      Length = 6,
      ExpiresOn = DateTime.Now.AddHours(1),
      MaximumAttempts = 5,
      CustomAttributes = [new CustomAttribute("Purpose", "MultiFactorAuthentication")]
    };

    User? user = null;
    if (withUser)
    {
      payload.User = _faker.Person.UserName;

      user = new(new UniqueName(new UniqueNameSettings(), payload.User), password: null, actorId, UserId.NewId(realmId));
      FoundUsers foundUsers = new(ById: null, user, ByEmailAddress: null, ByCustomIdentifier: null);
      _userManager.Setup(x => x.FindAsync(payload.User, _cancellationToken)).ReturnsAsync(foundUsers);
    }

    string passwordString = _faker.Random.String(payload.Length, minChar: '0', maxChar: '9');
    Base64Password password = new(passwordString);
    _passwordManager.Setup(x => x.Generate(payload.Characters, payload.Length, out passwordString)).Returns(password);

    OneTimePasswordDto dto = new();
    _oneTimePasswordQuerier.Setup(x => x.ReadAsync(It.IsAny<OneTimePassword>(), _cancellationToken)).ReturnsAsync(dto);

    CreateOneTimePassword command = new(payload);
    OneTimePasswordDto result = await _handler.HandleAsync(command, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(dto, result);
    Assert.Equal(passwordString, result.Password);

    _oneTimePasswordRepository.Verify(x => x.SaveAsync(
      It.Is<OneTimePassword>(y => y.CreatedBy == actorId && y.UpdatedBy == actorId
        && y.RealmId == realmId
        && (!id.HasValue || id.Value == y.EntityId)
        && (user == null ? y.UserId == null : y.UserId == user.Id)
        && y.ExpiresOn == payload.ExpiresOn
        && y.MaximumAttempts == payload.MaximumAttempts
        && y.CustomAttributes.Count == 1 && y.CustomAttributes.Single().Key.Value == "Purpose" && y.CustomAttributes.Single().Value == "MultiFactorAuthentication"),
      _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already used.")]
  public async Task Given_IdAlreadyUsed_When_HandleAsync_Then_IdAlreadyUsedException()
  {
    Base64Password password = new("457913");
    OneTimePassword oneTimePassword = new(password);
    _oneTimePasswordRepository.Setup(x => x.LoadAsync(oneTimePassword.Id, _cancellationToken)).ReturnsAsync(oneTimePassword);

    CreateOneTimePasswordPayload payload = new()
    {
      Id = oneTimePassword.EntityId,
      Characters = "0123456789",
      Length = 6
    };
    CreateOneTimePassword command = new(payload);

    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException<OneTimePassword>>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Null(exception.RealmId);
    Assert.Equal("OneTimePassword", exception.EntityType);
    Assert.Equal(payload.Id, exception.EntityId);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOneTimePasswordPayload payload = new()
    {
      Characters = string.Empty,
      Length = -1,
      ExpiresOn = DateTime.Now.AddDays(-1),
      MaximumAttempts = -1,
      CustomAttributes = [new CustomAttribute("123_Test", string.Empty)]
    };
    CreateOneTimePassword command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Characters");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "Length");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "FutureValidator" && e.PropertyName == "ExpiresOn.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "MaximumAttempts.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }
}
