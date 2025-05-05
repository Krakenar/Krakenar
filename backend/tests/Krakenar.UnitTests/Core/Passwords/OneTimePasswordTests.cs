using Bogus;
using Krakenar.Core.Passwords.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords;

[Trait(Traits.Category, Categories.Unit)]
public class OneTimePasswordTests
{
  private readonly Faker _faker = new();
  private readonly string _passwordString;
  private readonly Base64Password _password;
  private readonly OneTimePassword _oneTimePassword;

  public OneTimePasswordTests()
  {
    _passwordString = _faker.Random.String(6, minChar: '0', maxChar: '9');
    _password = new(_passwordString);
    _oneTimePassword = new(_password, expiresOn: DateTime.Now.AddHours(1), maximumAttempts: 5);
  }

  [Fact(DisplayName = "Delete: it should delete the One-Time Password (OTP).")]
  public void Given_OneTimePassword_When_Delete_Then_Deleted()
  {
    _oneTimePassword.Delete();
    Assert.True(_oneTimePassword.IsDeleted);

    _oneTimePassword.ClearChanges();
    _oneTimePassword.Delete();
    Assert.False(_oneTimePassword.HasChanges);
    Assert.Empty(_oneTimePassword.Changes);
  }

  [Fact(DisplayName = "IsExpired: it should return false when the One-Time Password (OTP) is not expired.")]
  public void Given_NotExpired_When_IsExpired_Then_FalseReturned()
  {
    Assert.False(_oneTimePassword.IsExpired());
    Assert.False(_oneTimePassword.IsExpired(DateTime.Now.AddMinutes(5)));
  }

  [Fact(DisplayName = "IsExpired: it should return true when the One-Time Password (OTP) is expired.")]
  public void Given_Expired_When_IsExpired_Then_TrueReturned()
  {
    Assert.True(_oneTimePassword.IsExpired(DateTime.Now.AddHours(1)));
    Assert.True(_oneTimePassword.IsExpired(DateTime.Now.AddDays(1)));
  }

  [Fact(DisplayName = "It should construct the correct One-Time Password (OTP) from default.")]
  public void Given_DefaultArguments_When_ctor_Then_Constructed()
  {
    OneTimePassword oneTimePassword = new(_password);

    Assert.Null(oneTimePassword.CreatedBy);
    Assert.Null(oneTimePassword.UpdatedBy);
    Assert.Null(oneTimePassword.RealmId);
    Assert.NotEqual(Guid.Empty, oneTimePassword.EntityId);
    Assert.Null(oneTimePassword.UserId);
    Assert.Null(oneTimePassword.ExpiresOn);
    Assert.Null(oneTimePassword.MaximumAttempts);
    Assert.Equal(0, oneTimePassword.AttemptCount);
    Assert.False(oneTimePassword.HasValidationSucceeded);
    Assert.Null(oneTimePassword.ExpiresOn);
    Assert.Empty(oneTimePassword.CustomAttributes);

    FieldInfo? field = oneTimePassword.GetType().GetField("_password", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);

    Password? value = field.GetValue(oneTimePassword) as Password;
    Assert.NotNull(value);
    Assert.Same(_password, value);
  }

  [Fact(DisplayName = "It should construct the correct One-Time Password (OTP).")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    DateTime expiresOn = DateTime.Now.AddHours(1);
    int maximumAttempts = 5;
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    ActorId actorId = ActorId.NewId();
    OneTimePasswordId oneTimePasswordId = OneTimePasswordId.NewId(realmId: null);

    OneTimePassword oneTimePassword = new(_password, expiresOn, maximumAttempts, user, actorId, oneTimePasswordId);

    Assert.Equal(expiresOn, oneTimePassword.ExpiresOn);
    Assert.Equal(maximumAttempts, oneTimePassword.MaximumAttempts);
    Assert.Equal(user.Id, oneTimePassword.UserId);
    Assert.Equal(actorId, oneTimePassword.CreatedBy);
    Assert.Equal(actorId, oneTimePassword.UpdatedBy);
    Assert.Equal(expiresOn, oneTimePassword.ExpiresOn);
    Assert.Equal(oneTimePasswordId, oneTimePassword.Id);
    Assert.Equal(0, oneTimePassword.AttemptCount);
    Assert.False(oneTimePassword.HasValidationSucceeded);
    Assert.Empty(oneTimePassword.CustomAttributes);

    FieldInfo? field = oneTimePassword.GetType().GetField("_password", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);

    Password? value = field.GetValue(oneTimePassword) as Password;
    Assert.NotNull(value);
    Assert.Same(_password, value);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the expiration is in the past.")]
  public void Given_ExpiredOnPast_When_ctor_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new OneTimePassword(_password, expiresOn: DateTime.Now.AddDays(-1)));
    Assert.StartsWith("The expiration date and time must be set in the future.", exception.Message);
    Assert.Equal("expiresOn", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when maximum attempts are below 1.")]
  [InlineData(0)]
  [InlineData(-2)]
  public void Given_ZeroOrLessMaximumAttempts_When_ctor_Then_ArgumentOutOfRangeException(int maximumAttempts)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new OneTimePassword(_password, maximumAttempts: maximumAttempts));
    Assert.StartsWith("There should be at least one attempt to validate the One-Time Password (OTP).", exception.Message);
    Assert.Equal("maximumAttempts", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw RealmMismatchException when the user is in another realm.")]
  public void Given_AnotherRealm_When_ctor_Then_RealmMismatchException()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), userId: UserId.NewId(RealmId.NewId()));

    var exception = Assert.Throws<RealmMismatchException>(() => new OneTimePassword(_password, user: user));
    Assert.Null(exception.ExpectedRealmId);
    Assert.NotNull(user.RealmId);
    Assert.Equal(user.RealmId.Value.ToGuid(), exception.ActualRealmId);
    Assert.Equal("user", exception.ParamName);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should not do anything when the custom attribute could not be found.")]
  public void Given_NotFound_When_RemoveCustomAttribute_Then_Nothing()
  {
    _oneTimePassword.SetCustomAttribute(new Identifier("Test"), "Hello World!");
    _oneTimePassword.Update();
    _oneTimePassword.ClearChanges();

    _oneTimePassword.RemoveCustomAttribute(new Identifier("Purpose"));

    Assert.False(_oneTimePassword.HasChanges);
    Assert.Empty(_oneTimePassword.Changes);
    Assert.Single(_oneTimePassword.CustomAttributes);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute found.")]
  public void Given_Found_When_RemoveCustomAttribute_Then_Removed()
  {
    Identifier purpose = new("Purpose");
    _oneTimePassword.SetCustomAttribute(purpose, "MultiFactorAuthentication");
    _oneTimePassword.SetCustomAttribute(new Identifier("Test"), "Hello World!");
    _oneTimePassword.Update();
    _oneTimePassword.ClearChanges();

    _oneTimePassword.RemoveCustomAttribute(purpose);
    _oneTimePassword.Update();

    Assert.Contains(_oneTimePassword.Changes, change => change is OneTimePasswordUpdated updated && updated.CustomAttributes[purpose] == null);
    Assert.NotEqual(purpose, Assert.Single(_oneTimePassword.CustomAttributes).Key);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("Purpose");
    _oneTimePassword.SetCustomAttribute(key, "MultiFactorAuthenticatio");
    _oneTimePassword.Update();

    _oneTimePassword.SetCustomAttribute(key, value!);
    _oneTimePassword.Update();
    Assert.False(_oneTimePassword.CustomAttributes.ContainsKey(key));
    Assert.Contains(_oneTimePassword.Changes, change => change is OneTimePasswordUpdated updated && updated.CustomAttributes[key] == null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("Purpose");
    string value = "  MultiFactorAuthentication  ";

    _oneTimePassword.SetCustomAttribute(key, value);
    _oneTimePassword.Update();
    Assert.Equal(_oneTimePassword.CustomAttributes[key], value.Trim());
    Assert.Contains(_oneTimePassword.Changes, change => change is OneTimePasswordUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _oneTimePassword.ClearChanges();
    _oneTimePassword.SetCustomAttribute(key, value.Trim());
    _oneTimePassword.Update();
    Assert.False(_oneTimePassword.HasChanges);
    Assert.Empty(_oneTimePassword.Changes);
  }

  [Fact(DisplayName = "Update: it should update the One-Time Password (OTP).")]
  public void Given_Changes_When_Update_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _oneTimePassword.SetCustomAttribute(new Identifier("Test"), "Hello World!");
    _oneTimePassword.Update(actorId);
    Assert.Contains(_oneTimePassword.Changes, change => change is OneTimePasswordUpdated updated && updated.ActorId == actorId);

    _oneTimePassword.ClearChanges();
    _oneTimePassword.Update();
    Assert.False(_oneTimePassword.HasChanges);
    Assert.Empty(_oneTimePassword.Changes);
  }

  [Fact(DisplayName = "Validate: it should return false when the password is not valid.")]
  public void Given_NotValid_When_Validate_Then_ValidationFailed()
  {
    ActorId actorId = ActorId.NewId();
    string password = "invalid";
    Assert.False(_oneTimePassword.Validate(password, actorId));

    Assert.Contains(_oneTimePassword.Changes, change => change is OneTimePasswordValidationFailed succeeded && succeeded.ActorId == actorId);
    Assert.False(_oneTimePassword.HasValidationSucceeded);
    Assert.Equal(1, _oneTimePassword.AttemptCount);

    _oneTimePassword.ClearChanges();

    Assert.False(_oneTimePassword.Validate(password, actorId));
    Assert.Contains(_oneTimePassword.Changes, change => change is OneTimePasswordValidationFailed succeeded && succeeded.ActorId == actorId);
    Assert.False(_oneTimePassword.HasValidationSucceeded);
    Assert.Equal(2, _oneTimePassword.AttemptCount);
  }

  [Fact(DisplayName = "Validate: it should succeed when the password is valid.")]
  public void Given_Valid_When_Validate_Then_ValidationSucceed()
  {
    ActorId actorId = ActorId.NewId();
    _oneTimePassword.Validate(_passwordString, actorId);

    Assert.Contains(_oneTimePassword.Changes, change => change is OneTimePasswordValidationSucceeded succeeded && succeeded.ActorId == actorId);
    Assert.True(_oneTimePassword.HasValidationSucceeded);
    Assert.Equal(1, _oneTimePassword.AttemptCount);
  }

  [Fact(DisplayName = "Validate: it should throw MaximumAttemptsReachedException when the maximum attempt count is reached.")]
  public void Given_MaximumAttemptsReached_When_Validate_Then_MaximumAttemptsReachedException()
  {
    string password = "invalid";
    for (int i = 0; i < _oneTimePassword.MaximumAttempts; i++)
    {
      Assert.False(_oneTimePassword.Validate(password));
    }

    var exception = Assert.Throws<MaximumAttemptsReachedException>(() => _oneTimePassword.Validate(_passwordString));
    Assert.Equal(_oneTimePassword.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_oneTimePassword.EntityId, exception.OneTimePasswordId);
    Assert.Equal(_oneTimePassword.AttemptCount, exception.AttemptCount);
  }

  [Fact(DisplayName = "Validate: it should throw OneTimePasswordAlreadyUsedException when validation has already succeeded.")]
  public void Given_Succeeded_When_Validate_Then_OneTimePasswordAlreadyUsedException()
  {
    _oneTimePassword.Validate(_passwordString);

    var exception = Assert.Throws<OneTimePasswordAlreadyUsedException>(() => _oneTimePassword.Validate("invalid"));
    Assert.Equal(_oneTimePassword.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_oneTimePassword.EntityId, exception.OneTimePasswordId);

    Assert.True(_oneTimePassword.HasValidationSucceeded);
    Assert.Equal(1, _oneTimePassword.AttemptCount);
  }

  [Fact(DisplayName = "Validate: it should throw OneTimePasswordIsExpiredException when One-Time Password (OTP) has expired.")]
  public void Given_Expired_When_Validate_Then_OneTimePasswordIsExpiredException()
  {
    RealmId realmId = RealmId.NewId();
    OneTimePassword oneTimePassword = new(_password, expiresOn: DateTime.Now.AddMilliseconds(50), oneTimePasswordId: OneTimePasswordId.NewId(realmId));

    Thread.Sleep(50);

    var exception = Assert.Throws<OneTimePasswordIsExpiredException>(() => oneTimePassword.Validate(_passwordString));
    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal(oneTimePassword.EntityId, exception.OneTimePasswordId);
  }
}
