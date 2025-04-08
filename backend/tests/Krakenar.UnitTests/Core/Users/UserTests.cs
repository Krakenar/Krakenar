using Bogus;
using Krakenar.Core.Localization;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.Core.Sessions;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using TimeZone = Krakenar.Core.Localization.TimeZone;

namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class UserTests
{
  private readonly Faker _faker = new();

  private readonly User _user;

  public UserTests()
  {
    _user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
  }

  [Fact(DisplayName = "AddRole: it should add a role.")]
  public void Given_Role_When_AddRole_Then_RoleAdded()
  {
    Role role = new(new UniqueName(new UniqueNameSettings(), "admin"));

    _user.AddRole(role);
    Assert.Contains(role.Id, _user.Roles);
    Assert.Contains(_user.Changes, change => change is UserRoleAdded added && added.RoleId == role.Id);

    _user.ClearChanges();
    _user.AddRole(role);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  //[Fact(DisplayName = "AddRole: it should throw RealmMismatchException when the role is in another realm.")]
  //public void Given_DifferentRealms_When_AddRole_Then_RealmMismatchException()
  //{
  //  Role role = new(new UniqueName(new UniqueNameSettings(), "guest"), actorId: null, new RoleId(Guid.NewGuid(), RealmId.NewId()));

  //  var exception = Assert.Throws<RealmMismatchException>(() => _user.AddRole(role));
  //  Assert.Equal(_user.RealmId?.ToGuid(), exception.ExpectedRealmId);
  //  Assert.Equal(role.RealmId?.ToGuid(), exception.ActualRealmId);
  //  Assert.Equal("role", exception.PropertyName);
  //} // TODO(fpion): RealmMismatchException

  [Fact(DisplayName = "Authenticate: it should authenticate the user.")]
  public void Given_CorrectPassword_When_Authenticate_Then_Authenticated()
  {
    Assert.Null(_user.AuthenticatedOn);

    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);

    ActorId actorId = ActorId.NewId();
    _user.Authenticate(passwordString, actorId);

    Assert.NotNull(_user.AuthenticatedOn);
    Assert.Equal(DateTime.Now, _user.AuthenticatedOn.Value, TimeSpan.FromSeconds(1));
    Assert.Contains(_user.Changes, change => change is UserAuthenticated authenticated && authenticated.ActorId == actorId);
  }

  [Fact(DisplayName = "Authenticate: it should throw IncorrectUserPasswordException when the input password is not correct.")]
  public void Given_IncorrectPassword_When_Authenticate_Then_IncorrectUserPasswordException()
  {
    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);

    string attemptedPassword = passwordString[..^1];
    var exception = Assert.Throws<IncorrectUserPasswordException>(() => _user.Authenticate(attemptedPassword));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
    Assert.Equal(attemptedPassword, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "Authenticate: it should throw UserHasNoPasswordException when the user has no password.")]
  public void Given_NoPassword_When_Authenticate_Then_UserHasNoPasswordException()
  {
    var exception = Assert.Throws<UserHasNoPasswordException>(() => _user.Authenticate("Test123!"));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
  }

  [Fact(DisplayName = "Authenticate: it should throw UserIsDisabledException when the user is disabled.")]
  public void Given_Disabled_When_Authenticate_Then_UserIsDisabledException()
  {
    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);
    _user.Disable();

    var exception = Assert.Throws<UserIsDisabledException>(() => _user.Authenticate(passwordString));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
  }

  [Fact(DisplayName = "Birthdate: it should handle the updates correctly.")]
  public void Given_BirthdateUpdates_When_setBirthdate_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Birthdate = _user.Birthdate;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Birthdate = _faker.Person.DateOfBirth;
    _user.Update();
    Assert.NotNull(_user.Birthdate);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Birthdate?.Value == _user.Birthdate);
  }

  [Fact(DisplayName = "Birthdate: it should throw ArgumentOutOfRangeException when the value is not in the past.")]
  public void Given_NotInThePast_When_setBirthdate_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _user.Birthdate = DateTime.Now.AddMinutes(1));
    Assert.StartsWith("The value must be a date and time set in the past.", exception.Message);
    Assert.Equal("Birthdate", exception.ParamName);
  }

  [Theory(DisplayName = "ChangePassword: it should change the password of the user.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_CorrectPassword_When_ChangePassword_Then_PasswordChanged(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new ActorId(actorIdValue);

    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);

    Base64Password newPassword = new(_faker.Internet.Password());
    _user.ChangePassword(passwordString, newPassword, actorId);
    Assert.True(_user.HasPassword);
    Assert.Contains(_user.Changes, change => change is UserPasswordChanged changed && changed.ActorId == (actorId ?? new(_user.Id.Value)) && changed.Password.Equals(newPassword));
  }

  [Fact(DisplayName = "ChangePassword: it should throw IncorrectUserPasswordException when the input password is not correct.")]
  public void Given_IncorrectPassword_When_ChangePassword_Then_IncorrectUserPasswordException()
  {
    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);

    string attemptedPassword = passwordString[..^1];
    var exception = Assert.Throws<IncorrectUserPasswordException>(() => _user.ChangePassword(attemptedPassword, password));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
    Assert.Equal(attemptedPassword, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "ChangePassword: it should throw UserHasNoPasswordException when the user has no password.")]
  public void Given_NoPassword_When_ChangePassword_Then_UserHasNoPasswordException()
  {
    var exception = Assert.Throws<UserHasNoPasswordException>(() => _user.ChangePassword("Test123!", new Base64Password("Test123!")));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
  }

  [Fact(DisplayName = "ChangePassword: it should throw UserIsDisabledException when the user is disabled.")]
  public void Given_Disabled_When_ChangePassword_Then_UserIsDisabledException()
  {
    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);
    _user.Disable();

    var exception = Assert.Throws<UserIsDisabledException>(() => _user.ChangePassword(passwordString, password));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
  }

  [Theory(DisplayName = "ctor: it should construct the correct user.")]
  [InlineData(null, true)]
  [InlineData("SYSTEM", false)]
  public void Given_Parameters_When_ctor_Then_CorrectUserConstructed(string? actorIdValue, bool generateId)
  {
    ActorId? actorId = actorIdValue is null ? null : new(actorIdValue);
    UserId? id = generateId ? UserId.NewId(realmId: null) : null;

    Base64Password password = new("Test123!");
    User user = new(_user.UniqueName, password, actorId, id);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, user.Id);
    }
    else
    {
      Assert.Null(user.RealmId);
      Assert.NotEqual(Guid.Empty, user.EntityId);
    }

    Assert.Equal(actorId, user.CreatedBy);
    Assert.Equal(_user.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);

    FieldInfo? field = user.GetType().GetField("_password", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);
    Password? instance = field.GetValue(user) as Password;
    Assert.NotNull(instance);
    Assert.Same(password, instance);
  }

  [Fact(DisplayName = "Delete: it should delete the user.")]
  public void Given_User_When_Delete_Then_Deleted()
  {
    _user.Delete();
    Assert.True(_user.IsDeleted);

    _user.ClearChanges();
    _user.Delete();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "Disable: it should disable the user.")]
  public void Given_User_When_Disable_Then_Disabled()
  {
    ActorId actorId = ActorId.NewId();
    _user.Disable(actorId);
    Assert.True(_user.IsDisabled);
    Assert.Contains(_user.Changes, change => change is UserDisabled disabled && disabled.ActorId == actorId);

    _user.ClearChanges();
    _user.Disable();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "Enable: it should enable the user.")]
  public void Given_User_When_Enable_Then_Enabled()
  {
    _user.Disable();

    ActorId actorId = ActorId.NewId();
    _user.Enable(actorId);
    Assert.False(_user.IsDisabled);
    Assert.Contains(_user.Changes, change => change is UserEnabled disabled && disabled.ActorId == actorId);

    _user.ClearChanges();
    _user.Enable();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "FirstName: it should handle the updates correctly.")]
  public void Given_FirstNameUpdates_When_setFirstName_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.FirstName = _user.FirstName;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.FirstName = new PersonName(_faker.Person.FirstName);
    _user.Update();
    Assert.NotNull(_user.FirstName);
    Assert.Equal(_user.FirstName.Value, _user.FullName);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.FirstName?.Value == _user.FirstName && updated.FullName?.Value == _user.FirstName.Value);
  }

  [Fact(DisplayName = "Gender: it should handle the updates correctly.")]
  public void Given_GenderUpdates_When_setGender_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Gender = _user.Gender;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Gender = new Gender(_faker.Person.Gender.ToString());
    _user.Update();
    Assert.NotNull(_user.Gender);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Gender?.Value == _user.Gender);
  }

  [Fact(DisplayName = "IsConfirmed: it should return false when the user has no verified contact.")]
  public void Given_NoVerifiedContact_When_IsConfirmed_Then_FalseReturned()
  {
    Assert.False(_user.IsConfirmed);

    Address address = new("150 Saint-Catherine St W", "Montreal", "CA", "QC", "H2X 3Y2");
    _user.SetAddress(address);
    Assert.False(_user.IsConfirmed);
  }

  [Fact(DisplayName = "IsConfirmed: it should return true when the user has verified contacts.")]
  public void Given_VerifiedContacts_When_IsConfirmed_Then_TrueReturned()
  {
    _user.SetEmail(new Email(_faker.Person.Email, isVerified: true));
    _user.SetPhone(new Phone("+15148454636", "CA", extension: null, isVerified: true));
    Assert.True(_user.IsConfirmed);
  }

  [Fact(DisplayName = "It should have the correct IDs.")]
  public void Given_User_When_getIds_Then_CorrectIds()
  {
    UserId id = new(Guid.NewGuid(), RealmId.NewId());
    User user = new(_user.UniqueName, password: null, actorId: null, id);
    Assert.Equal(id, user.Id);
    Assert.Equal(id.RealmId, user.RealmId);
    Assert.Equal(id.EntityId, user.EntityId);
  }

  [Fact(DisplayName = "LastName: it should handle the updates correctly.")]
  public void Given_LastNameUpdates_When_setLastName_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.LastName = _user.LastName;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.LastName = new PersonName(_faker.Person.LastName);
    _user.Update();
    Assert.NotNull(_user.LastName);
    Assert.Equal(_user.LastName.Value, _user.FullName);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.LastName?.Value == _user.LastName && updated.FullName?.Value == _user.LastName.Value);
  }

  [Fact(DisplayName = "Locale: it should handle the updates correctly.")]
  public void Given_LocaleUpdates_When_setLocale_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Locale = _user.Locale;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Locale = new Locale(_faker.Locale);
    _user.Update();
    Assert.NotNull(_user.Locale);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Locale?.Value == _user.Locale);
  }

  [Fact(DisplayName = "MiddleName: it should handle the updates correctly.")]
  public void Given_MiddleNameUpdates_When_setMiddleName_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.MiddleName = _user.MiddleName;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.MiddleName = new PersonName(_faker.Name.FirstName());
    _user.Update();
    Assert.NotNull(_user.MiddleName);
    Assert.Equal(_user.MiddleName.Value, _user.FullName);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.MiddleName?.Value == _user.MiddleName && updated.FullName?.Value == _user.MiddleName.Value);
  }

  [Fact(DisplayName = "Nickname: it should handle the updates correctly.")]
  public void Given_NicknameUpdates_When_setNickname_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Nickname = _user.Nickname;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Nickname = new PersonName(_faker.Name.LastName());
    _user.Update();
    Assert.NotNull(_user.Nickname);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Nickname?.Value == _user.Nickname);
  }

  [Fact(DisplayName = "Picture: it should handle the updates correctly.")]
  public void Given_PictureUpdates_When_setPicture_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Picture = _user.Picture;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Picture = new Url($"https://www.{_faker.Person.Avatar}");
    _user.Update();
    Assert.NotNull(_user.Picture);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Picture?.Value == _user.Picture);
  }

  [Fact(DisplayName = "Profile: it should handle the updates correctly.")]
  public void Given_ProfileUpdates_When_setProfile_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Profile = _user.Profile;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Profile = new Url($"https://www.{_faker.Internet.DomainName()}/members/{_faker.Person.UserName}");
    _user.Update();
    Assert.NotNull(_user.Profile);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Profile?.Value == _user.Profile);
  }

  [Fact(DisplayName = "RemoveCustomAttribute: it should remove the custom attribute.")]
  public void Given_CustomAttributes_When_RemoveCustomAttribute_Then_CustomAttributeRemoved()
  {
    Identifier key = new("HealthInsuranceNumber");
    _user.SetCustomAttribute(key, _faker.Person.BuildHealthInsuranceNumber());
    _user.Update();

    _user.RemoveCustomAttribute(key);
    _user.Update();
    Assert.False(_user.CustomAttributes.ContainsKey(key));
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.CustomAttributes[key] is null);

    _user.ClearChanges();
    _user.RemoveCustomAttribute(key);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "RemoveCustomIdentifier: it should remove the custom attribute.")]
  public void Given_CustomIdentifiers_When_RemoveCustomIdentifier_Then_CustomIdentifierRemoved()
  {
    Identifier key = new("Google");
    _user.SetCustomIdentifier(key, new CustomIdentifier("1234567890"));
    _user.Update();

    ActorId actorId = ActorId.NewId();
    _user.RemoveCustomIdentifier(key, actorId);
    Assert.Empty(_user.CustomIdentifiers);
    Assert.Contains(_user.Changes, change => change is UserIdentifierRemoved removed && removed.ActorId == actorId && removed.Key == key);

    _user.ClearChanges();
    _user.RemoveCustomIdentifier(key);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "RemovePassword: it should remove the password.")]
  public void Given_Password_When_RemovePassword_Then_Removed()
  {
    Base64Password password = new(_faker.Internet.Password());
    _user.SetPassword(password);
    Assert.True(_user.HasPassword);

    ActorId actorId = ActorId.NewId();
    _user.RemovePassword(actorId);
    Assert.False(_user.HasPassword);
    Assert.Contains(_user.Changes, change => change is UserPasswordRemoved removed && removed.ActorId == actorId);

    _user.ClearChanges();
    _user.RemovePassword();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "RemoveRole: it should remove a role.")]
  public void Given_Role_When_RemoveRole_Then_RoleRemoved()
  {
    Role role = new(new UniqueName(new UniqueNameSettings(), "client"));
    _user.AddRole(role);
    Assert.True(_user.HasRole(role));

    _user.RemoveRole(role);
    Assert.Contains(_user.Changes, change => change is UserRoleRemoved removed && removed.RoleId == role.Id);

    _user.ClearChanges();
    _user.RemoveRole(role);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
    Assert.False(_user.HasRole(role));
  }

  [Fact(DisplayName = "ResetPassword: it should reset the user password when it is not disabled.")]
  public void Given_NotDisabled_When_ResetPassword_Then_PasswordReset()
  {
    Base64Password password = new(_faker.Internet.Password());
    ActorId actorId = ActorId.NewId();
    _user.ResetPassword(password, actorId);

    Assert.True(_user.HasPassword);
    Assert.Contains(_user.Changes, change => change is UserPasswordReset reset && reset.ActorId == actorId && reset.Password.Equals(password));
  }

  [Fact(DisplayName = "ResetPassword: it should throw UserIsDisabledException when the user is disabled.")]
  public void Given_Disabled_When_ResetPassword_Then_UserIsDisabledException()
  {
    _user.Disable();

    Base64Password password = new(_faker.Internet.Password());
    var exception = Assert.Throws<UserIsDisabledException>(() => _user.ResetPassword(password));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
  }

  [Fact(DisplayName = "SetAddress: it should set the user address.")]
  public void Given_User_When_SetAddress_Then_AddressChanged()
  {
    Address address = new("150 Saint-Catherine St W", "Montreal", "CA", "QC", "H2X 3Y2");
    ActorId actorId = ActorId.NewId();
    _user.SetAddress(address, actorId);
    Assert.Same(_user.Address, address);
    Assert.Contains(_user.Changes, change => change is UserAddressChanged changed && changed.Address is not null && changed.Address.Equals(address) && changed.ActorId == actorId);

    _user.ClearChanges();
    _user.SetAddress(address);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.SetAddress(address: null, actorId);
    Assert.Null(_user.Address);
    Assert.Contains(_user.Changes, change => change is UserAddressChanged changed && changed.Address is null && changed.ActorId == actorId);
  }

  [Theory(DisplayName = "SetCustomAttribute: it should remove the custom attribute when the value is null, empty or white-space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyValue_When_SetCustomAttribute_Then_CustomAttributeRemoved(string? value)
  {
    Identifier key = new("HealthInsuranceNumber");
    _user.SetCustomAttribute(key, _faker.Person.BuildHealthInsuranceNumber());
    _user.Update();

    _user.SetCustomAttribute(key, value!);
    _user.Update();
    Assert.False(_user.CustomAttributes.ContainsKey(key));
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.CustomAttributes[key] is null);
  }

  [Fact(DisplayName = "SetCustomAttribute: it should set a custom attribute.")]
  public void Given_CustomAttribute_When_SetCustomAttribute_Then_CustomAttributeSet()
  {
    Identifier key = new("HealthInsuranceNumber");
    string value = $"  {_faker.Person.BuildHealthInsuranceNumber()}  ";

    _user.SetCustomAttribute(key, value);
    _user.Update();
    Assert.Equal(_user.CustomAttributes[key], value.Trim());
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.CustomAttributes[key] == value.Trim());

    _user.ClearChanges();
    _user.SetCustomAttribute(key, value.Trim());
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "SetCustomIdentifier: it should set a custom identifier.")]
  public void Given_CustomIdentifier_When_SetCustomIdentifier_Then_CustomIdentifierSet()
  {
    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    ActorId actorId = ActorId.NewId();

    _user.SetCustomIdentifier(key, value, actorId);
    Assert.Equal(value, _user.CustomIdentifiers[key]);
    Assert.Contains(_user.Changes, change => change is UserIdentifierChanged changed && changed.ActorId == actorId && changed.Key == key && changed.Value == value);

    _user.ClearChanges();
    _user.SetCustomIdentifier(key, value);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Fact(DisplayName = "SetEmail: it should set the user email.")]
  public void Given_User_When_SetEmail_Then_EmailChanged()
  {
    Email email = new(_faker.Person.Email, isVerified: true);
    ActorId actorId = ActorId.NewId();
    _user.SetEmail(email, actorId);
    Assert.Same(_user.Email, email);
    Assert.Contains(_user.Changes, change => change is UserEmailChanged changed && changed.Email is not null && changed.Email.Equals(email) && changed.ActorId == actorId);

    _user.ClearChanges();
    _user.SetEmail(email);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.SetEmail(email: null, actorId);
    Assert.Null(_user.Email);
    Assert.Contains(_user.Changes, change => change is UserEmailChanged changed && changed.Email is null && changed.ActorId == actorId);
  }

  [Fact(DisplayName = "SetPassword: it should update the user password.")]
  public void Given_Password_When_SetPassword_Then_PasswordUpdated()
  {
    Base64Password password = new(_faker.Internet.Password());
    ActorId actorId = ActorId.NewId();
    _user.SetPassword(password, actorId);

    Assert.True(_user.HasPassword);
    Assert.Contains(_user.Changes, change => change is UserPasswordUpdated updated && updated.ActorId == actorId && updated.Password.Equals(password));
  }

  [Fact(DisplayName = "SetPhone: it should set the user phone.")]
  public void Given_User_When_SetPhone_Then_PhoneChanged()
  {
    Phone phone = new("+15148454636", "CA", extension: null, isVerified: true);
    ActorId actorId = ActorId.NewId();
    _user.SetPhone(phone, actorId);
    Assert.Same(_user.Phone, phone);
    Assert.Contains(_user.Changes, change => change is UserPhoneChanged changed && changed.Phone is not null && changed.Phone.Equals(phone) && changed.ActorId == actorId);

    _user.ClearChanges();
    _user.SetPhone(phone);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.SetPhone(phone: null, actorId);
    Assert.Null(_user.Phone);
    Assert.Contains(_user.Changes, change => change is UserPhoneChanged changed && changed.Phone is null && changed.ActorId == actorId);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle the updated correctly.")]
  public void Given_UniqueNameUpdates_When_setSetUniqueName_Then_UpdatesHandledCorrectly()
  {
    UniqueName uniqueName = new(new UniqueNameSettings(), "member");
    _user.SetUniqueName(uniqueName);
    Assert.Contains(_user.Changes, change => change is UserUniqueNameChanged changed && changed.UniqueName == uniqueName);

    _user.ClearChanges();
    _user.SetUniqueName(uniqueName);
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);
  }

  [Theory(DisplayName = "SignIn: it should sign-in the user.")]
  [InlineData(null, false)]
  [InlineData("SYSTEM", true)]
  public void Given_CorrectPassword_When_SignIn_Then_SignedIn(string? actorIdValue, bool isPersistent)
  {
    ActorId? actorId = actorIdValue is null ? null : new ActorId(actorIdValue);
    RealmId realmId = RealmId.NewId();

    User user = new(_user.UniqueName, userId: UserId.NewId(realmId));

    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    user.SetPassword(password);

    Base64Password? secret = isPersistent ? new(_faker.Internet.Password()) : null;
    Guid? sessionId = isPersistent ? Guid.NewGuid() : null;
    Session session = user.SignIn(passwordString, secret, actorId, sessionId);
    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(session.CreatedOn, user.AuthenticatedOn.Value);
    Assert.Contains(user.Changes, change => change is UserSignedIn signedIn && signedIn.ActorId == (actorId ?? new(user.Id.Value)) && signedIn.OccurredOn == session.CreatedOn);

    Assert.Equal(realmId, session.RealmId);
    if (sessionId.HasValue)
    {
      Assert.Equal(sessionId.Value, session.EntityId);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, session.EntityId);
    }
    Assert.Equal(actorId ?? new(user.Id.Value), session.CreatedBy);
    Assert.Equal(actorId ?? new(user.Id.Value), session.UpdatedBy);
    Assert.Equal(DateTime.Now, session.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(user.Id, session.UserId);
    Assert.Equal(secret is not null, session.IsPersistent);
    Assert.True(session.IsActive);
  }

  [Fact(DisplayName = "SignIn: it should throw IncorrectUserPasswordException when the input password is not correct.")]
  public void Given_IncorrectPassword_When_SignIn_Then_IncorrectUserPasswordException()
  {
    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);

    string attemptedPassword = passwordString[..^1];
    var exception = Assert.Throws<IncorrectUserPasswordException>(() => _user.SignIn(attemptedPassword));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
    Assert.Equal(attemptedPassword, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "SignIn: it should throw UserHasNoPasswordException when the user has no password.")]
  public void Given_NoPassword_When_SignIn_Then_UserHasNoPasswordException()
  {
    var exception = Assert.Throws<UserHasNoPasswordException>(() => _user.SignIn("Test123!"));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
  }

  [Fact(DisplayName = "SignIn: it should throw UserIsDisabledException when the user is disabled.")]
  public void Given_Disabled_When_SignIn_Then_UserIsDisabledException()
  {
    string passwordString = _faker.Internet.Password();
    Base64Password password = new(passwordString);
    _user.SetPassword(password);
    _user.Disable();

    var exception = Assert.Throws<UserIsDisabledException>(() => _user.SignIn(passwordString));
    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_user.EntityId, exception.UserId);
  }

  [Fact(DisplayName = "TimeZone: it should handle the updates correctly.")]
  public void Given_TimeZoneUpdates_When_setTimeZone_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.TimeZone = _user.TimeZone;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.TimeZone = new TimeZone("America/New_York");
    _user.Update();
    Assert.NotNull(_user.TimeZone);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.TimeZone?.Value == _user.TimeZone);
  }

  [Theory(DisplayName = "Update: it should update the user.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_UserUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(actorIdValue);

    _user.ClearChanges();
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), _faker.Person.BuildHealthInsuranceNumber());
    _user.Update(actorId);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }

  [Fact(DisplayName = "Website: it should handle the updates correctly.")]
  public void Given_WebsiteUpdates_When_setWebsite_Then_UpdatesHandledCorrectly()
  {
    _user.ClearChanges();

    _user.Website = _user.Website;
    _user.Update();
    Assert.False(_user.HasChanges);
    Assert.Empty(_user.Changes);

    _user.Website = new Url($"https://www.{_faker.Person.Website}");
    _user.Update();
    Assert.NotNull(_user.Website);
    Assert.True(_user.HasChanges);
    Assert.Contains(_user.Changes, change => change is UserUpdated updated && updated.Website?.Value == _user.Website);
  }
}
