using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Users;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceUserHandlerTests
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

  private readonly CreateOrReplaceUserHandler _handler;

  private readonly UniqueNameSettings _uniqueNameSettings = new();
  private readonly PasswordSettings _passwordSettings = new();

  public CreateOrReplaceUserHandlerTests()
  {
    _handler = new(_addressHelper, _applicationContext.Object, _passwordService.Object, _roleManager.Object, _userManager.Object, _userQuerier.Object, _userRepository.Object);

    _applicationContext.SetupGet(x => x.UniqueNameSettings).Returns(_uniqueNameSettings);
    _applicationContext.SetupGet(x => x.PasswordSettings).Returns(_passwordSettings);
  }

  [Theory(DisplayName = "It should create a new user.")]
  [InlineData(null)]
  [InlineData("b099efaa-0941-4f64-a87e-7a43dd934a2e")]
  public async Task Given_NotExists_When_HandleAsync_Then_Created(string? idValue)
  {
    Guid? id = idValue is null ? null : Guid.Parse(idValue);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Base64Password password = new(PasswordString);
    _passwordService.Setup(x => x.ValidateAndHash(PasswordString, null)).Returns(password);

    Role admin = new(new UniqueName(_uniqueNameSettings, "admin"), actorId, RoleId.NewId(realmId));
    Dictionary<string, Role> roles = new()
    {
      ["  AdmIn  "] = admin
    };
    _roleManager.Setup(x => x.FindAsync(It.Is<IEnumerable<string>>(y => y.SequenceEqual(roles.Keys)), "Roles", _cancellationToken)).ReturnsAsync(roles);

    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = _faker.Person.UserName,
      Password = new ChangePasswordPayload(PasswordString),
      Address = new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC"),
      Email = new EmailPayload(_faker.Person.Email, isVerified: true),
      Phone = new PhonePayload("+15148454636", "CA", "123456", isVerified: true),
      FirstName = _faker.Person.FirstName,
      MiddleName = " Mido ",
      LastName = _faker.Person.LastName,
      Nickname = "  Oska  ",
      Birthdate = _faker.Person.DateOfBirth,
      Gender = _faker.Person.Gender.ToString(),
      Locale = _faker.Locale,
      TimeZone = "America/New_York",
      Picture = _faker.Person.Avatar,
      Profile = $"https://www.{_faker.Person.Website}/profile",
      Website = $"https://www.{_faker.Person.Website}"
    };
    payload.CustomAttributes.Add(new CustomAttribute("HealthInsuranceNumber", _faker.Person.BuildHealthInsuranceNumber()));
    payload.Roles.Add("  AdmIn  ");

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(It.IsAny<User>(), _cancellationToken)).ReturnsAsync(dto);

    User? user = null;
    _userManager.Setup(x => x.SaveAsync(It.IsAny<User>(), _cancellationToken)).Callback<User, CancellationToken>((r, _) => user = r);

    CreateOrReplaceUser command = new(id, payload, Version: null);
    CreateOrReplaceUserResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.User);
    Assert.Same(dto, result.User);

    Assert.NotNull(user);
    Assert.Equal(actorId, user.CreatedBy);
    Assert.Equal(actorId, user.UpdatedBy);
    Assert.Equal(realmId, user.RealmId);
    Assert.Equal(payload.UniqueName, user.UniqueName.Value);
    Assert.Contains(user.Changes, change => change is UserCreated created && created.Password == password);
    Assert.False(user.IsDisabled);
    Assert.Equal(new Address(payload.Address, payload.Address.IsVerified), user.Address);
    Assert.Equal(new Email(payload.Email, payload.Email.IsVerified), user.Email);
    Assert.Equal(new Phone(payload.Phone, payload.Phone.IsVerified), user.Phone);
    Assert.True(user.IsConfirmed);
    Assert.Equal(payload.FirstName, user.FirstName?.Value);
    Assert.Equal(payload.MiddleName.Trim(), user.MiddleName?.Value);
    Assert.Equal(payload.LastName, user.LastName?.Value);
    Assert.Equal(payload.Nickname.Trim(), user.Nickname?.Value);
    Assert.Equal(payload.Birthdate, user.Birthdate);
    Assert.Equal(payload.Gender.ToLowerInvariant(), user.Gender?.Value);
    Assert.Equal(payload.Locale, user.Locale?.Code);
    Assert.Equal(payload.TimeZone, user.TimeZone?.Id);
    Assert.Equal(payload.Picture, user.Picture?.Value);
    Assert.Equal(payload.Profile, user.Profile?.Value);
    Assert.Equal(payload.Website, user.Website?.Value);

    Assert.Equal(payload.CustomAttributes.Count, user.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, user.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    Assert.Single(user.Roles);
    Assert.Contains(admin.Id, user.Roles);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, user.EntityId);

      _userRepository.Verify(x => x.LoadAsync(It.Is<UserId>(i => i.RealmId == realmId && i.EntityId == id.Value), _cancellationToken), Times.Once);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, user.EntityId);
    }
  }

  [Fact(DisplayName = "It should replace an existing user.")]
  public async Task Given_Found_When_HandleAsync_Then_Replaced()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    User user = new(new UniqueName(_uniqueNameSettings, "user"), password: null, actorId);
    user.SetCustomAttribute(new Identifier("EmployeeId"), Guid.NewGuid().ToString());
    user.Update(actorId);
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    Base64Password password = new(PasswordString);
    _passwordService.Setup(x => x.ValidateAndHash(PasswordString, null)).Returns(password);

    Role admin = new(new UniqueName(_uniqueNameSettings, "admin"), actorId);
    Dictionary<string, Role> roles = new()
    {
      ["  AdmIn  "] = admin
    };
    _roleManager.Setup(x => x.FindAsync(It.Is<IEnumerable<string>>(y => y.SequenceEqual(roles.Keys)), "Roles", _cancellationToken)).ReturnsAsync(roles);

    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = _faker.Person.UserName,
      Password = new ChangePasswordPayload(PasswordString),
      IsDisabled = true,
      Address = new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC"),
      Email = new EmailPayload(_faker.Person.Email, isVerified: true),
      Phone = new PhonePayload("+15148454636", "CA", "123456", isVerified: true),
      FirstName = _faker.Person.FirstName,
      MiddleName = " Mido ",
      LastName = _faker.Person.LastName,
      Nickname = "  Oska  ",
      Birthdate = _faker.Person.DateOfBirth,
      Gender = _faker.Person.Gender.ToString(),
      Locale = _faker.Locale,
      TimeZone = "America/New_York",
      Picture = $"https://www.{_faker.Person.Avatar}",
      Profile = $"https://www.{_faker.Person.Website}/profile",
      Website = $"https://www.{_faker.Person.Website}"
    };
    payload.CustomAttributes.Add(new CustomAttribute("HealthInsuranceNumber", _faker.Person.BuildHealthInsuranceNumber()));
    payload.Roles.Add("  AdmIn  ");

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceUser command = new(user.EntityId, payload, Version: null);
    CreateOrReplaceUserResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.User);
    Assert.Same(dto, result.User);

    Assert.NotNull(user);
    Assert.Equal(actorId, user.CreatedBy);
    Assert.Equal(actorId, user.UpdatedBy);
    Assert.Null(user.RealmId);
    Assert.Equal(payload.UniqueName, user.UniqueName.Value);
    Assert.Contains(user.Changes, change => change is UserPasswordUpdated updated && updated.Password == password);
    Assert.True(user.IsDisabled);
    Assert.Equal(new Address(payload.Address, payload.Address.IsVerified), user.Address);
    Assert.Equal(new Email(payload.Email, payload.Email.IsVerified), user.Email);
    Assert.Equal(new Phone(payload.Phone, payload.Phone.IsVerified), user.Phone);
    Assert.True(user.IsConfirmed);
    Assert.Equal(payload.FirstName, user.FirstName?.Value);
    Assert.Equal(payload.MiddleName.Trim(), user.MiddleName?.Value);
    Assert.Equal(payload.LastName, user.LastName?.Value);
    Assert.Equal(payload.Nickname.Trim(), user.Nickname?.Value);
    Assert.Equal(payload.Birthdate, user.Birthdate);
    Assert.Equal(payload.Gender.ToLowerInvariant(), user.Gender?.Value);
    Assert.Equal(payload.Locale, user.Locale?.Code);
    Assert.Equal(payload.TimeZone, user.TimeZone?.Id);
    Assert.Equal(payload.Picture, user.Picture?.Value);
    Assert.Equal(payload.Profile, user.Profile?.Value);
    Assert.Equal(payload.Website, user.Website?.Value);

    Assert.Equal(payload.CustomAttributes.Count, user.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, user.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    Assert.Single(user.Roles);
    Assert.Contains(admin.Id, user.Roles);

    _userRepository.Verify(x => x.LoadAsync(It.IsAny<UserId>(), It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Never);
    _userManager.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the user does not exist.")]
  public async Task Given_NotFound_Then_HandleAsync_Then_NullReturned()
  {
    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = "not_found"
    };
    CreateOrReplaceUser command = new(Guid.Empty, payload, Version: -1);
    CreateOrReplaceUserResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.User);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = "invalid!",
      Password = new ChangePasswordPayload("", "    "),
      Address = new AddressPayload(
        RandomStringGenerator.GetString(999),
        RandomStringGenerator.GetString(999),
        "CA",
        "Z0Z0Z0",
        "CQ"),
      Email = new EmailPayload("aa@@bb..cc"),
      Phone = new PhonePayload(
        RandomStringGenerator.GetString(999),
        "CA",
        "ext. 12345"),
      FirstName = RandomStringGenerator.GetString(999),
      MiddleName = RandomStringGenerator.GetString(999),
      LastName = RandomStringGenerator.GetString(999),
      Nickname = RandomStringGenerator.GetString(999),
      Birthdate = DateTime.Now.AddYears(1),
      Gender = RandomStringGenerator.GetString(999),
      Locale = "invalid",
      TimeZone = "invalid",
      Picture = "invalid",
      Profile = "invalid",
      Website = "invalid"
    };
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", string.Empty));

    CreateOrReplaceUser command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(28, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password.Current");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordTooShort" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUniqueChars" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresNonAlphanumeric" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresLower" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUpper" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresDigit" && e.PropertyName == "Password.New");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Address.Street");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Address.Locality");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PostalCodeValidator" && e.PropertyName == "Address.PostalCode");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "RegionValidator" && e.PropertyName == "Address.Region");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EmailValidator" && e.PropertyName == "Email.Address");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Phone.Number");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PhoneValidator" && e.PropertyName == "Phone");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "FirstName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "MiddleName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "LastName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Nickname");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PastValidator" && e.PropertyName == "Birthdate.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Gender");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Locale");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "TimeZoneValidator" && e.PropertyName == "TimeZone");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Picture");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Profile");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Website");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }

  [Fact(DisplayName = "It should update an existing user.")]
  public async Task Given_Found_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    string passwordString = "Test123!";
    Base64Password currentPassword = new(passwordString);

    Role admin = new(new UniqueName(_uniqueNameSettings, "admin"), actorId);
    Role editor = new(new UniqueName(_uniqueNameSettings, "editor"), actorId);
    Role guest = new(new UniqueName(_uniqueNameSettings, "guest"), actorId);
    Role member = new(new UniqueName(_uniqueNameSettings, "member"), actorId);
    Role publisher = new(new UniqueName(_uniqueNameSettings, "publisher"), actorId);
    Dictionary<string, Role> roles = new()
    {
      ["  AdmIn  "] = admin,
      ["guest"] = guest,
      ["publisher"] = publisher
    };
    _roleManager.Setup(x => x.FindAsync(It.Is<IEnumerable<string>>(y => y.SequenceEqual(roles.Keys)), "Roles", _cancellationToken)).ReturnsAsync(roles);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName), currentPassword, actorId);
    string hin = _faker.Person.BuildHealthInsuranceNumber();
    user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), hin);
    string employeeId = Guid.NewGuid().ToString();
    user.SetCustomAttribute(new Identifier("EmployeeId"), employeeId);
    user.SetCustomAttribute(new Identifier("IAL"), "1.0");
    user.SetCustomAttribute(new Identifier("Department"), "Engineering");
    user.AddRole(editor, actorId);
    user.AddRole(guest, actorId);
    user.AddRole(publisher, actorId);
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    User reference = new(user.UniqueName, currentPassword, actorId, user.Id);
    foreach (KeyValuePair<Identifier, string> customAttribute in user.CustomAttributes)
    {
      reference.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    reference.Update(actorId);
    reference.AddRole(editor, actorId);
    reference.AddRole(guest, actorId);
    reference.AddRole(publisher, actorId);
    _userRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    PersonName middleName = new("Mido");
    user.MiddleName = middleName;
    PersonName nickname = new("Oska");
    user.Nickname = nickname;
    user.RemoveCustomAttribute(new Identifier("EmployeeId"));
    user.SetCustomAttribute(new Identifier("EmployeeNo"), "110874");
    user.SetCustomAttribute(new Identifier("IAL"), "2.0");
    user.Update(actorId);
    user.AddRole(member, actorId);
    user.RemoveRole(guest, actorId);

    Base64Password password = new(PasswordString);
    _passwordService.Setup(x => x.ValidateAndHash(PasswordString, null)).Returns(password);

    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = _faker.Person.UserName,
      Password = new ChangePasswordPayload(PasswordString, passwordString),
      IsDisabled = true,
      Address = new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC"),
      Email = new EmailPayload(_faker.Person.Email, isVerified: true),
      Phone = new PhonePayload("+15148454636", "CA", "123456", isVerified: true),
      FirstName = _faker.Person.FirstName,
      LastName = _faker.Person.LastName,
      Birthdate = _faker.Person.DateOfBirth,
      Gender = _faker.Person.Gender.ToString(),
      Locale = _faker.Locale,
      TimeZone = "America/New_York",
      Picture = $"https://www.{_faker.Person.Avatar}",
      Profile = $"https://www.{_faker.Person.Website}/profile",
      Website = $"https://www.{_faker.Person.Website}"
    };
    payload.CustomAttributes.Add(new CustomAttribute("HealthInsuranceNumber", hin));
    payload.CustomAttributes.Add(new CustomAttribute("EmployeeId", employeeId));
    payload.CustomAttributes.Add(new CustomAttribute("JobTitle", "Engineer"));
    payload.CustomAttributes.Add(new CustomAttribute("IAL", "3.0"));
    payload.Roles.Add("  AdmIn  ");
    payload.Roles.Add("guest");
    payload.Roles.Add("publisher");

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceUser command = new(user.EntityId, payload, reference.Version);
    CreateOrReplaceUserResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.User);
    Assert.Same(dto, result.User);

    Assert.NotNull(user);
    Assert.Equal(actorId, user.CreatedBy);
    Assert.Equal(actorId, user.UpdatedBy);
    Assert.Null(user.RealmId);
    Assert.Equal(payload.UniqueName, user.UniqueName.Value);
    Assert.Contains(user.Changes, change => change is UserPasswordChanged changed && changed.Password == password);
    Assert.True(user.IsDisabled);
    Assert.Equal(new Address(payload.Address, payload.Address.IsVerified), user.Address);
    Assert.Equal(new Email(payload.Email, payload.Email.IsVerified), user.Email);
    Assert.Equal(new Phone(payload.Phone, payload.Phone.IsVerified), user.Phone);
    Assert.True(user.IsConfirmed);
    Assert.Equal(payload.FirstName, user.FirstName?.Value);
    Assert.Equal(middleName, user.MiddleName);
    Assert.Equal(payload.LastName, user.LastName?.Value);
    Assert.Equal(nickname, user.Nickname);
    Assert.Equal(payload.Birthdate, user.Birthdate);
    Assert.Equal(payload.Gender.ToLowerInvariant(), user.Gender?.Value);
    Assert.Equal(payload.Locale, user.Locale?.Code);
    Assert.Equal(payload.TimeZone, user.TimeZone?.Id);
    Assert.Equal(payload.Picture, user.Picture?.Value);
    Assert.Equal(payload.Profile, user.Profile?.Value);
    Assert.Equal(payload.Website, user.Website?.Value);

    Assert.Equal(4, user.CustomAttributes.Count);
    Assert.Equal("110874", user.CustomAttributes[new Identifier("EmployeeNo")]);
    Assert.Equal(hin, user.CustomAttributes[new Identifier("HealthInsuranceNumber")]);
    Assert.Equal("3.0", user.CustomAttributes[new Identifier("IAL")]);
    Assert.Equal("Engineer", user.CustomAttributes[new Identifier("JobTitle")]);

    Assert.Equal(3, user.Roles.Count);
    Assert.Contains(admin.Id, user.Roles);
    Assert.Contains(member.Id, user.Roles);
    Assert.Contains(publisher.Id, user.Roles);

    _userRepository.Verify(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken), Times.Once);
    _userManager.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }
}
