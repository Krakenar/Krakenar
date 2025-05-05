using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Passwords;
using Krakenar.Core.Passwords.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Moq;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ValidateOneTimePasswordHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IOneTimePasswordQuerier> _oneTimePasswordQuerier = new();
  private readonly Mock<IOneTimePasswordRepository> _oneTimePasswordRepository = new();

  private readonly ValidateOneTimePasswordHandler _handler;

  public ValidateOneTimePasswordHandlerTests()
  {
    _handler = new(_applicationContext.Object, _oneTimePasswordQuerier.Object, _oneTimePasswordRepository.Object);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password (OTP) could not be found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ValidateOneTimePasswordPayload payload = new()
    {
      Password = _faker.Random.String(6, minChar: '0', maxChar: '9')
    };
    ValidateOneTimePassword command = new(Guid.NewGuid(), payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should save the One-Time Password (OTP) and throw IncorrectOneTimePasswordPasswordException when the password is not valid.")]
  public async Task Given_InvalidPassword_When_HandleAsync_Then_SavedAndIncorrectOneTimePasswordPasswordException()
  {
    string passwordString = _faker.Random.String(6, minChar: '0', maxChar: '9');
    Base64Password password = new(passwordString);
    OneTimePassword oneTimePassword = new(password);
    _oneTimePasswordRepository.Setup(x => x.LoadAsync(oneTimePassword.Id, _cancellationToken)).ReturnsAsync(oneTimePassword);

    ValidateOneTimePasswordPayload payload = new()
    {
      Password = "invalid"
    };
    ValidateOneTimePassword command = new(oneTimePassword.EntityId, payload);
    var exception = await Assert.ThrowsAsync<IncorrectOneTimePasswordPasswordException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(oneTimePassword.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(oneTimePassword.EntityId, exception.OneTimePasswordId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);

    Assert.Contains(oneTimePassword.Changes, change => change is OneTimePasswordValidationFailed);
    Assert.False(oneTimePassword.HasValidationSucceeded);
    Assert.Equal(1, oneTimePassword.AttemptCount);

    _oneTimePasswordRepository.Verify(x => x.SaveAsync(oneTimePassword, _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    ValidateOneTimePasswordPayload payload = new()
    {
      Password = string.Empty,
      CustomAttributes = [new CustomAttribute("123_Test", string.Empty)]
    };
    ValidateOneTimePassword command = new(Guid.NewGuid(), payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }

  [Fact(DisplayName = "It should validate a valid One-Time Password (OTP).")]
  public async Task Given_ValidPassword_When_HandleAsync_Then_Validated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), userId: UserId.NewId(realmId));

    string passwordString = _faker.Random.String(6, minChar: '0', maxChar: '9');
    Base64Password password = new(passwordString);
    OneTimePassword oneTimePassword = new(password, expiresOn: DateTime.Now.AddHours(1), maximumAttempts: 5, user, actorId, OneTimePasswordId.NewId(realmId));
    string additionalInformation = $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}";
    oneTimePassword.SetCustomAttribute(new Identifier("AdditionalInformation"), additionalInformation);
    oneTimePassword.SetCustomAttribute(new Identifier("Purpose"), "mfa");
    oneTimePassword.Update();
    _oneTimePasswordRepository.Setup(x => x.LoadAsync(oneTimePassword.Id, _cancellationToken)).ReturnsAsync(oneTimePassword);

    OneTimePasswordDto dto = new();
    _oneTimePasswordQuerier.Setup(x => x.ReadAsync(oneTimePassword, _cancellationToken)).ReturnsAsync(dto);

    ValidateOneTimePasswordPayload payload = new()
    {
      Password = passwordString,
      CustomAttributes =
      [
        new CustomAttribute("IpAddress", _faker.Internet.Ip()),
        new CustomAttribute("Purpose", "MultiFactorAuthentication")
      ]
    };
    ValidateOneTimePassword command = new(oneTimePassword.EntityId, payload);
    OneTimePasswordDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.Contains(oneTimePassword.Changes, change => change is OneTimePasswordValidationSucceeded succeeded && succeeded.ActorId == actorId);
    Assert.Equal(actorId, oneTimePassword.CreatedBy);
    Assert.Equal(actorId, oneTimePassword.UpdatedBy);
    Assert.True(oneTimePassword.HasValidationSucceeded);
    Assert.Equal(1, oneTimePassword.AttemptCount);

    Assert.Equal(3, oneTimePassword.CustomAttributes.Count);
    Assert.Contains(oneTimePassword.CustomAttributes, c => c.Key.Value == "AdditionalInformation" && c.Value == additionalInformation);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Contains(oneTimePassword.CustomAttributes, c => c.Key.Value == customAttribute.Key && c.Value == customAttribute.Value);
    }

    _oneTimePasswordRepository.Verify(x => x.SaveAsync(oneTimePassword, _cancellationToken), Times.Once());
  }
}
