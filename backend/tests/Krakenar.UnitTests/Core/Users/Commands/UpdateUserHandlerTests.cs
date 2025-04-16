using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Users;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using Role = Krakenar.Core.Roles.Role;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateUserHandlerTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly AddressHelper _addressHelper = new();
  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IPasswordService> _passwordService = new();
  private readonly Mock<IRoleManager> _roleManager = new();
  private readonly Mock<IUserManager> _userManager = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly UpdateUserHandler _handler;

  private readonly UniqueNameSettings _uniqueNameSettings = new();
  private readonly PasswordSettings _passwordSettings = new();

  public UpdateUserHandlerTests()
  {
    _handler = new(_addressHelper, _applicationContext.Object, _passwordService.Object, _roleManager.Object, _userManager.Object, _userQuerier.Object, _userRepository.Object);

    _applicationContext.SetupGet(x => x.UniqueNameSettings).Returns(_uniqueNameSettings);
    _applicationContext.SetupGet(x => x.PasswordSettings).Returns(_passwordSettings);
  }

  [Fact(DisplayName = "It should return null when the user was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    UpdateUserPayload payload = new();
    UpdateUser command = new(Guid.Empty, payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateUserPayload payload = new()
    {
      UniqueName = "invalid!",
      Password = new ChangePasswordPayload("", "    "),
      Address = new Contracts.Change<AddressPayload>(new AddressPayload(
        RandomStringGenerator.GetString(999),
        RandomStringGenerator.GetString(999),
        "invalid",
        RandomStringGenerator.GetString(999),
        RandomStringGenerator.GetString(999))),
      Email = new Contracts.Change<EmailPayload>(new EmailPayload("aa@@bb..cc")),
      Phone = new Contracts.Change<PhonePayload>(new PhonePayload(
        RandomStringGenerator.GetString(999),
        "invalid_country_code",
        "invalid_extension")),
      FirstName = new Contracts.Change<string>(RandomStringGenerator.GetString(999)),
      MiddleName = new Contracts.Change<string>(RandomStringGenerator.GetString(999)),
      LastName = new Contracts.Change<string>(RandomStringGenerator.GetString(999)),
      Nickname = new Contracts.Change<string>(RandomStringGenerator.GetString(999)),
      Birthdate = new Contracts.Change<DateTime?>(DateTime.Now.AddYears(1)),
      Gender = new Contracts.Change<string>(RandomStringGenerator.GetString(999)),
      Locale = new Contracts.Change<string>("invalid"),
      TimeZone = new Contracts.Change<string>("invalid"),
      Picture = new Contracts.Change<string>("invalid"),
      Profile = new Contracts.Change<string>("invalid"),
      Website = new Contracts.Change<string>("invalid")
    };
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", string.Empty));
    payload.Roles.Add(new RoleChange("role", (CollectionAction)(-1)));

    UpdateUser command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(32, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password.Current");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordTooShort" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUniqueChars" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresNonAlphanumeric" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresLower" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUpper" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresDigit" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Address.Value.Street");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Address.Value.Locality");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Address.Value.PostalCode");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Address.Value.Region");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CountryValidator" && e.PropertyName == "Address.Value.Country");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EmailValidator" && e.PropertyName == "Email.Value.Address");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "ExactLengthValidator" && e.PropertyName == "Phone.Value.CountryCode");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Phone.Value.Number");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Phone.Value.Extension");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PhoneValidator" && e.PropertyName == "Phone.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "FirstName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "MiddleName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "LastName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Nickname.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PastValidator" && e.PropertyName == "Birthdate.Value.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Gender.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Locale.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "TimeZoneValidator" && e.PropertyName == "TimeZone.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Picture.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Profile.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Website.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Roles[0].Action");
  }

  [Theory(DisplayName = "It should update the user.")]
  [InlineData(null)]
  [InlineData("Test123!")]
  public async Task Given_User_When_HandleAsync_Then_Updated(string? currentPassword)
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Role admin = new(new UniqueName(_uniqueNameSettings, "admin"), actorId, RoleId.NewId(realmId));
    Role editor = new(new UniqueName(_uniqueNameSettings, "editor"), actorId, RoleId.NewId(realmId));

    Dictionary<string, Role> roles = new()
    {
      ["  ADMIN  "] = admin,
      ["editor"] = editor
    };
    _roleManager.Setup(x => x.FindAsync(It.Is<IEnumerable<string>>(y => y.SequenceEqual(roles.Keys)), "Roles", _cancellationToken)).ReturnsAsync(roles);

    Password? password = currentPassword is null ? null : new Base64Password(currentPassword);
    PersonName nickname = new("Oska");
    User user = new(new UniqueName(_uniqueNameSettings, _faker.Internet.UserName()), password, actorId, UserId.NewId(realmId))
    {
      Nickname = nickname
    };
    user.AddRole(admin, actorId);
    user.AddRole(editor, actorId);
    user.SetCustomAttribute(new Identifier("EmployeeId"), Guid.NewGuid().ToString());
    string hin = _faker.Person.BuildHealthInsuranceNumber();
    user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), hin);
    user.SetCustomAttribute(new Identifier("IAL"), "1.0");
    user.SetPhone(new Phone("+15148454636", "CA", "123456", isVerified: true), actorId);
    user.Update(actorId);
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);
    long version = user.Version;

    Base64Password newPassword = new(PasswordString);
    _passwordService.Setup(x => x.ValidateAndHash(PasswordString, null)).Returns(newPassword);

    UpdateUserPayload payload = new()
    {
      UniqueName = _faker.Person.UserName,
      Password = new ChangePasswordPayload(PasswordString, currentPassword),
      IsDisabled = true,
      Address = new Contracts.Change<AddressPayload>(new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC")),
      Email = new Contracts.Change<EmailPayload>(new EmailPayload(_faker.Person.Email, isVerified: true)),
      Phone = new Contracts.Change<PhonePayload>(),
      FirstName = new Contracts.Change<string>(_faker.Person.FirstName),
      LastName = new Contracts.Change<string>(_faker.Person.LastName),
      Birthdate = new Contracts.Change<DateTime?>(_faker.Person.DateOfBirth),
      Gender = new Contracts.Change<string>(_faker.Person.Gender.ToString()),
      Locale = new Contracts.Change<string>(_faker.Locale),
      TimeZone = new Contracts.Change<string>("America/New_York"),
      Picture = new Contracts.Change<string>(_faker.Person.Avatar),
      Profile = new Contracts.Change<string>($"https://www.{_faker.Person.Website}/profile"),
      Website = new Contracts.Change<string>($"https://www.{_faker.Person.Website}")
    };
    payload.CustomAttributes.Add(new CustomAttribute("EmployeeId", string.Empty));
    payload.CustomAttributes.Add(new CustomAttribute("EmployeeNo", "110879"));
    payload.CustomAttributes.Add(new CustomAttribute("IAL", "2.0"));
    payload.Roles.Add(new RoleChange("  ADMIN  ", CollectionAction.Add));
    payload.Roles.Add(new RoleChange("editor", CollectionAction.Remove));

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    UpdateUser command = new(user.EntityId, payload);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _userManager.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);

    Assert.Equal(version + 8, user.Version);
    Assert.Equal(actorId, user.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(1));

    Assert.Equal(payload.UniqueName, user.UniqueName.Value);
    Assert.True(user.HasPassword);
    if (currentPassword is null)
    {
      Assert.Contains(user.Changes, change => change is UserPasswordUpdated updated && updated.Password == newPassword);
    }
    else
    {
      Assert.Contains(user.Changes, change => change is UserPasswordChanged changed && changed.Password == newPassword);
    }
    Assert.Equal(payload.IsDisabled, user.IsDisabled);

    Assert.NotNull(payload.Address.Value);
    Assert.Equal(new Address(payload.Address.Value, payload.Address.Value.IsVerified), user.Address);
    Assert.NotNull(payload.Email.Value);
    Assert.Equal(new Email(payload.Email.Value, payload.Email.Value.IsVerified), user.Email);
    Assert.Null(user.Phone);
    Assert.True(user.IsConfirmed);

    Assert.Equal(payload.FirstName.Value, user.FirstName?.Value);
    Assert.Null(user.MiddleName);
    Assert.Equal(payload.LastName.Value, user.LastName?.Value);
    Assert.Equal(nickname, user.Nickname);
    Assert.Equal(payload.Birthdate.Value, user.Birthdate);
    Assert.Equal(payload.Gender.Value?.ToLowerInvariant(), user.Gender?.Value);
    Assert.Equal(payload.Locale.Value, user.Locale?.Code);
    Assert.Equal(payload.TimeZone.Value, user.TimeZone?.Id);
    Assert.Equal(payload.Picture.Value, user.Picture?.Value);
    Assert.Equal(payload.Profile.Value, user.Profile?.Value);
    Assert.Equal(payload.Website.Value, user.Website?.Value);

    Assert.Equal(3, user.CustomAttributes.Count);
    Assert.Equal("110879", user.CustomAttributes[new Identifier("EmployeeNo")]);
    Assert.Equal(hin, user.CustomAttributes[new Identifier("HealthInsuranceNumber")]);
    Assert.Equal("2.0", user.CustomAttributes[new Identifier("IAL")]);

    Assert.Single(user.Roles);
    Assert.Contains(admin.Id, user.Roles);
  }
}
