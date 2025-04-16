using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Passwords;
using Krakenar.Core.Roles;
using Krakenar.Core.Sessions;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Commands;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Address = Krakenar.Core.Users.Address;
using CustomIdentifier = Krakenar.Core.CustomIdentifier;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using Email = Krakenar.Core.Users.Email;
using Identifier = Krakenar.Core.Identifier;
using Phone = Krakenar.Core.Users.Phone;
using Role = Krakenar.Core.Roles.Role;
using Session = Krakenar.Core.Sessions.Session;
using SessionEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Session;
using User = Krakenar.Core.Users.User;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Users;

[Trait(Traits.Category, Categories.Integration)]
public class UserIntegrationTests : IntegrationTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly IPasswordService _passwordService;
  private readonly IRoleRepository _roleRepository;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;
  private readonly IUserService _userService;

  private readonly User _user;

  public UserIntegrationTests() : base()
  {
    _passwordService = ServiceProvider.GetRequiredService<IPasswordService>();
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
    _userService = ServiceProvider.GetRequiredService<IUserService>();

    Password password = _passwordService.ValidateAndHash(PasswordString, Realm.PasswordSettings);
    _user = new User(new UniqueName(Realm.UniqueNameSettings, Faker.Person.UserName), password, actorId: null, UserId.NewId(Realm.Id));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _userRepository.SaveAsync(_user);
  }

  // TODO(fpion): search

  [Fact(DisplayName = "It should authenticate the user.")]
  public async Task Given_Valid_When_Authenticate_Then_Authenticated()
  {
    AuthenticateUserPayload payload = new(_user.UniqueName.Value, PasswordString);
    UserDto user = await _userService.AuthenticateAsync(payload);

    Assert.Equal(RealmDto, user.Realm);
    Assert.Equal(payload.User, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should create a new user.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync(admin);

    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = Faker.Internet.UserName(),
      Password = new ChangePasswordPayload(PasswordString),
      Address = new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC"),
      Email = new EmailPayload(Faker.Person.Email, isVerified: true),
      Phone = new PhonePayload("+15148454636", "CA", "123456", isVerified: true),
      FirstName = Faker.Person.FirstName,
      MiddleName = " Mido ",
      LastName = Faker.Person.LastName,
      Nickname = "  Oska  ",
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString(),
      Locale = Faker.Locale,
      TimeZone = "America/New_York",
      Picture = $"https://www.{Faker.Person.Avatar}",
      Profile = $"https://www.{Faker.Person.Website}/profile",
      Website = $"https://www.{Faker.Person.Website}"
    };
    payload.CustomAttributes.Add(new CustomAttribute("HealthInsuranceNumber", Faker.Person.BuildHealthInsuranceNumber()));
    payload.Roles.Add("  AdmIn  ");

    Guid id = Guid.NewGuid();
    CreateOrReplaceUserResult result = await _userService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    UserDto? user = result.User;
    Assert.NotNull(user);
    Assert.Equal(id, user.Id);
    Assert.Equal(6, user.Version);
    Assert.Equal(Actor, user.CreatedBy);
    Assert.Equal(DateTime.UtcNow, user.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, user.CreatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(RealmDto, user.Realm);

    Assert.Equal(payload.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.Equal(Actor, user.PasswordChangedBy);
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Equal(DateTime.UtcNow, user.PasswordChangedOn.Value, TimeSpan.FromSeconds(10));
    Assert.Null(user.DisabledBy);
    Assert.Null(user.DisabledOn);
    Assert.False(user.IsDisabled);

    Assert.NotNull(user.Address);
    Assert.Equal(payload.Address.Street, user.Address.Street);
    Assert.Equal(payload.Address.Locality, user.Address.Locality);
    Assert.Equal(payload.Address.PostalCode, user.Address.PostalCode);
    Assert.Equal(payload.Address.Region, user.Address.Region);
    Assert.Equal(payload.Address.Country, user.Address.Country);
    Assert.Equal(new Address(payload.Address).ToString(), user.Address.Formatted);
    Assert.False(user.Address.IsVerified);
    Assert.Null(user.Address.VerifiedBy);
    Assert.Null(user.Address.VerifiedOn);

    Assert.NotNull(user.Email);
    Assert.Equal(payload.Email.Address, user.Email.Address);
    Assert.True(user.Email.IsVerified);
    Assert.Equal(Actor, user.Email.VerifiedBy);
    Assert.NotNull(user.Email.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Email.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    Assert.NotNull(user.Phone);
    Assert.Equal(payload.Phone.CountryCode, user.Phone.CountryCode);
    Assert.Equal(payload.Phone.Number, user.Phone.Number);
    Assert.Equal(payload.Phone.Extension, user.Phone.Extension);
    Assert.Equal(payload.Phone.FormatToE164(), user.Phone.E164Formatted);
    Assert.True(user.Phone.IsVerified);
    Assert.Equal(Actor, user.Phone.VerifiedBy);
    Assert.NotNull(user.Phone.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Phone.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    Assert.True(user.IsConfirmed);

    Assert.Equal(payload.FirstName, user.FirstName);
    Assert.Equal(payload.MiddleName.Trim(), user.MiddleName);
    Assert.Equal(payload.LastName, user.LastName);
    Assert.Equal(payload.Nickname.Trim(), user.Nickname);
    Assert.NotNull(user.Birthdate);
    Assert.Equal(payload.Birthdate.Value.ToUniversalTime(), user.Birthdate.Value, TimeSpan.FromSeconds(10));
    Assert.Equal(payload.Gender.ToLowerInvariant(), user.Gender);
    Assert.Equal(payload.Locale, user.Locale?.Code);
    Assert.Equal(payload.TimeZone, user.TimeZone);
    Assert.Equal(payload.Picture, user.Picture);
    Assert.Equal(payload.Profile, user.Profile);
    Assert.Equal(payload.Website, user.Website);
    Assert.Equal(payload.CustomAttributes, user.CustomAttributes);

    Assert.Single(user.Roles);
    Assert.Contains(user.Roles, r => r.Id == admin.EntityId);
  }

  [Fact(DisplayName = "It should delete the user and its sessions.")]
  public async Task Given_User_When_Delete_Then_DeletedWithSessions()
  {
    Session session = new(_user);
    await _sessionRepository.SaveAsync(session);

    UserDto? dto = await _userService.DeleteAsync(_user.EntityId);
    Assert.NotNull(dto);
    Assert.Equal(_user.EntityId, dto.Id);
    Assert.Equal(_user.Version, dto.Version);

    Assert.Empty(await KrakenarContext.Users.AsNoTracking().Where(x => x.StreamId == _user.Id.Value).ToArrayAsync());
    Assert.Empty(await KrakenarContext.Sessions.AsNoTracking().Where(x => x.StreamId == session.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the user by custom identifier.")]
  public async Task Given_CustomIdentifier_When_Read_Then_Found()
  {
    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    _user.SetCustomIdentifier(key, value, ActorId);
    await _userRepository.SaveAsync(_user);

    UserDto? user = await _userService.ReadAsync(id: null, uniqueName: null, new CustomIdentifierDto(key.Value, value.Value));
    Assert.NotNull(user);
    Assert.Equal(_user.EntityId, user.Id);
  }

  [Fact(DisplayName = "It should read the user by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    UserDto? user = await _userService.ReadAsync(_user.EntityId);
    Assert.NotNull(user);
    Assert.Equal(_user.EntityId, user.Id);
  }

  [Fact(DisplayName = "It should read the user by email address.")]
  public async Task Given_EmailAddress_When_Read_Then_Found()
  {
    Email email = new(Faker.Person.Email);
    _user.SetEmail(email, ActorId);
    await _userRepository.SaveAsync(_user);

    UserDto? user = await _userService.ReadAsync(id: null, email.Address);
    Assert.NotNull(user);
    Assert.Equal(_user.EntityId, user.Id);
  }

  [Fact(DisplayName = "It should read the user by unique name.")]
  public async Task Given_UniqueName_When_Read_Then_Found()
  {
    UserDto? user = await _userService.ReadAsync(id: null, _user.UniqueName.Value);
    Assert.NotNull(user);
    Assert.Equal(_user.EntityId, user.Id);
  }

  [Fact(DisplayName = "It should remove a user identifier.")]
  public async Task Given_UserFound_When_RemoveIdentifier_Then_IdentifierRemoved()
  {
    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    _user.SetCustomIdentifier(key, value, ActorId);
    await _userRepository.SaveAsync(_user);

    UserDto? dto = await _userService.RemoveIdentifierAsync(_user.EntityId, key.Value);
    Assert.NotNull(dto);

    Assert.Equal(_user.EntityId, dto.Id);
    Assert.Equal(_user.Version + 1, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(1));
    Assert.Empty(dto.CustomIdentifiers);
  }

  [Fact(DisplayName = "It should replace an existing user.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    Role guest = new(new UniqueName(Realm.UniqueNameSettings, "guest"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync([admin, guest]);

    _user.AddRole(guest, ActorId);
    await _userRepository.SaveAsync(_user);

    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = Faker.Person.UserName,
      Password = new ChangePasswordPayload(PasswordString),
      IsDisabled = true,
      Address = new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC"),
      Email = new EmailPayload(Faker.Person.Email, isVerified: true),
      Phone = new PhonePayload("+15148454636", "CA", "123456", isVerified: true),
      FirstName = Faker.Person.FirstName,
      MiddleName = " Mido ",
      LastName = Faker.Person.LastName,
      Nickname = "  Oska  ",
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString(),
      Locale = Faker.Locale,
      TimeZone = "America/New_York",
      Picture = $"https://www.{Faker.Person.Avatar}",
      Profile = $"https://www.{Faker.Person.Website}/profile",
      Website = $"https://www.{Faker.Person.Website}"
    };
    payload.CustomAttributes.Add(new CustomAttribute("HealthInsuranceNumber", Faker.Person.BuildHealthInsuranceNumber()));
    payload.Roles.Add("  AdmIn  ");

    CreateOrReplaceUserResult result = await _userService.CreateOrReplaceAsync(payload, _user.EntityId);
    Assert.False(result.Created);

    UserDto? user = result.User;
    Assert.NotNull(user);
    Assert.Equal(Actor, user.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.Equal(Actor, user.PasswordChangedBy);
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Equal(DateTime.UtcNow, user.PasswordChangedOn.Value, TimeSpan.FromSeconds(10));
    Assert.True(user.IsDisabled);
    Assert.Equal(Actor, user.DisabledBy);
    Assert.NotNull(user.DisabledOn);
    Assert.Equal(DateTime.UtcNow, user.DisabledOn.Value, TimeSpan.FromSeconds(10));

    Assert.NotNull(user.Address);
    Assert.Equal(payload.Address.Street, user.Address.Street);
    Assert.Equal(payload.Address.Locality, user.Address.Locality);
    Assert.Equal(payload.Address.PostalCode, user.Address.PostalCode);
    Assert.Equal(payload.Address.Region, user.Address.Region);
    Assert.Equal(payload.Address.Country, user.Address.Country);
    Assert.Equal(new Address(payload.Address).ToString(), user.Address.Formatted);
    Assert.False(user.Address.IsVerified);
    Assert.Null(user.Address.VerifiedBy);
    Assert.Null(user.Address.VerifiedOn);

    Assert.NotNull(user.Email);
    Assert.Equal(payload.Email.Address, user.Email.Address);
    Assert.True(user.Email.IsVerified);
    Assert.Equal(Actor, user.Email.VerifiedBy);
    Assert.NotNull(user.Email.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Email.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    Assert.NotNull(user.Phone);
    Assert.Equal(payload.Phone.CountryCode, user.Phone.CountryCode);
    Assert.Equal(payload.Phone.Number, user.Phone.Number);
    Assert.Equal(payload.Phone.Extension, user.Phone.Extension);
    Assert.Equal(payload.Phone.FormatToE164(), user.Phone.E164Formatted);
    Assert.True(user.Phone.IsVerified);
    Assert.Equal(Actor, user.Phone.VerifiedBy);
    Assert.NotNull(user.Phone.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Phone.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    Assert.True(user.IsConfirmed);

    Assert.Equal(payload.FirstName, user.FirstName);
    Assert.Equal(payload.MiddleName.Trim(), user.MiddleName);
    Assert.Equal(payload.LastName, user.LastName);
    Assert.Equal(payload.Nickname.Trim(), user.Nickname);
    Assert.NotNull(user.Birthdate);
    Assert.Equal(payload.Birthdate.Value.ToUniversalTime(), user.Birthdate.Value, TimeSpan.FromSeconds(10));
    Assert.Equal(payload.Gender.ToLowerInvariant(), user.Gender);
    Assert.Equal(payload.Locale, user.Locale?.Code);
    Assert.Equal(payload.TimeZone, user.TimeZone);
    Assert.Equal(payload.Picture, user.Picture);
    Assert.Equal(payload.Profile, user.Profile);
    Assert.Equal(payload.Website, user.Website);
    Assert.Equal(payload.CustomAttributes, user.CustomAttributes);

    Assert.Single(user.Roles);
    Assert.Contains(user.Roles, r => r.Id == admin.EntityId);
  }

  [Fact(DisplayName = "It should replace an existing user from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    Role editor = new(new UniqueName(Realm.UniqueNameSettings, "editor"), ActorId, RoleId.NewId(Realm.Id));
    Role guest = new(new UniqueName(Realm.UniqueNameSettings, "guest"), ActorId, RoleId.NewId(Realm.Id));
    Role member = new(new UniqueName(Realm.UniqueNameSettings, "member"), ActorId, RoleId.NewId(Realm.Id));
    Role publisher = new(new UniqueName(Realm.UniqueNameSettings, "publisher"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync([admin, editor, guest, member, publisher]);

    string hin = Faker.Person.BuildHealthInsuranceNumber();
    _user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), hin);
    string employeeId = Guid.NewGuid().ToString();
    _user.SetCustomAttribute(new Identifier("EmployeeId"), employeeId);
    _user.SetCustomAttribute(new Identifier("IAL"), "1.0");
    _user.SetCustomAttribute(new Identifier("Department"), "Engineering");
    _user.AddRole(editor, ActorId);
    _user.AddRole(guest, ActorId);
    _user.AddRole(publisher, ActorId);
    _user.Update(ActorId);
    long version = _user.Version;

    PersonName middleName = new("Mido");
    _user.MiddleName = middleName;
    PersonName nickname = new("Oska");
    _user.Nickname = nickname;
    _user.RemoveCustomAttribute(new Identifier("EmployeeId"));
    _user.SetCustomAttribute(new Identifier("EmployeeNo"), "110874");
    _user.SetCustomAttribute(new Identifier("IAL"), "2.0");
    _user.Update(ActorId);
    _user.AddRole(member, ActorId);
    _user.RemoveRole(guest, ActorId);
    await _userRepository.SaveAsync(_user);

    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = Faker.Person.UserName,
      Password = new ChangePasswordPayload("Test123!", PasswordString),
      IsDisabled = true,
      Address = new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC"),
      Email = new EmailPayload(Faker.Person.Email, isVerified: true),
      Phone = new PhonePayload("+15148454636", "CA", "123456", isVerified: true),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString(),
      Locale = Faker.Locale,
      TimeZone = "America/New_York",
      Picture = $"https://www.{Faker.Person.Avatar}",
      Profile = $"https://www.{Faker.Person.Website}/profile",
      Website = $"https://www.{Faker.Person.Website}"
    };
    payload.CustomAttributes.Add(new CustomAttribute("HealthInsuranceNumber", hin));
    payload.CustomAttributes.Add(new CustomAttribute("EmployeeId", employeeId));
    payload.CustomAttributes.Add(new CustomAttribute("JobTitle", "Engineer"));
    payload.CustomAttributes.Add(new CustomAttribute("IAL", "3.0"));
    payload.Roles.Add("  AdmIn  ");
    payload.Roles.Add("guest");
    payload.Roles.Add("publisher");

    CreateOrReplaceUserResult result = await _userService.CreateOrReplaceAsync(payload, _user.EntityId, version);
    Assert.False(result.Created);

    UserDto? user = result.User;
    Assert.NotNull(user);
    Assert.Equal(_user.EntityId, user.Id);
    Assert.Equal(Actor, user.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.Equal(Actor, user.PasswordChangedBy);
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Equal(DateTime.UtcNow, user.PasswordChangedOn.Value, TimeSpan.FromSeconds(10));
    Assert.True(user.IsDisabled);
    Assert.Equal(Actor, user.DisabledBy);
    Assert.NotNull(user.DisabledOn);
    Assert.Equal(DateTime.UtcNow, user.DisabledOn.Value, TimeSpan.FromSeconds(10));

    Assert.NotNull(user.Address);
    Assert.Equal(payload.Address.Street, user.Address.Street);
    Assert.Equal(payload.Address.Locality, user.Address.Locality);
    Assert.Equal(payload.Address.PostalCode, user.Address.PostalCode);
    Assert.Equal(payload.Address.Region, user.Address.Region);
    Assert.Equal(payload.Address.Country, user.Address.Country);
    Assert.Equal(new Address(payload.Address).ToString(), user.Address.Formatted);
    Assert.False(user.Address.IsVerified);
    Assert.Null(user.Address.VerifiedBy);
    Assert.Null(user.Address.VerifiedOn);

    Assert.NotNull(user.Email);
    Assert.Equal(payload.Email.Address, user.Email.Address);
    Assert.True(user.Email.IsVerified);
    Assert.Equal(Actor, user.Email.VerifiedBy);
    Assert.NotNull(user.Email.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Email.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    Assert.NotNull(user.Phone);
    Assert.Equal(payload.Phone.CountryCode, user.Phone.CountryCode);
    Assert.Equal(payload.Phone.Number, user.Phone.Number);
    Assert.Equal(payload.Phone.Extension, user.Phone.Extension);
    Assert.Equal(payload.Phone.FormatToE164(), user.Phone.E164Formatted);
    Assert.True(user.Phone.IsVerified);
    Assert.Equal(Actor, user.Phone.VerifiedBy);
    Assert.NotNull(user.Phone.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Phone.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    Assert.True(user.IsConfirmed);

    Assert.Equal(payload.FirstName, user.FirstName);
    Assert.Equal(middleName.Value, user.MiddleName);
    Assert.Equal(payload.LastName, user.LastName);
    Assert.Equal(nickname.Value, user.Nickname);
    Assert.NotNull(user.Birthdate);
    Assert.Equal(payload.Birthdate.Value.AsUniversalTime(), user.Birthdate.Value, TimeSpan.FromSeconds(10));
    Assert.Equal(payload.Gender.ToLowerInvariant(), user.Gender);
    Assert.Equal(payload.Locale, user.Locale?.Code);
    Assert.Equal(payload.TimeZone, user.TimeZone);
    Assert.Equal(payload.Picture, user.Picture);
    Assert.Equal(payload.Profile, user.Profile);
    Assert.Equal(payload.Website, user.Website);

    Assert.Equal(4, user.CustomAttributes.Count);
    Assert.Contains(user.CustomAttributes, c => c.Key == "EmployeeNo" && c.Value == "110874");
    Assert.Contains(user.CustomAttributes, c => c.Key == "HealthInsuranceNumber" && c.Value == hin);
    Assert.Contains(user.CustomAttributes, c => c.Key == "IAL" && c.Value == "3.0");
    Assert.Contains(user.CustomAttributes, c => c.Key == "JobTitle" && c.Value == "Engineer");

    Assert.Equal(3, user.Roles.Count);
    Assert.Contains(user.Roles, r => r.Id == admin.EntityId);
    Assert.Contains(user.Roles, r => r.Id == member.EntityId);
    Assert.Contains(user.Roles, r => r.Id == publisher.EntityId);
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _userService.ReadAsync(Guid.Empty, "not-found", new CustomIdentifierDto("Google", "1234567890")));
  }

  [Fact(DisplayName = "It should reset the user password.")]
  public async Task Given_UserFound_When_ResetPassword_Then_PasswordReset()
  {
    ResetUserPasswordPayload payload = new("N3wP@s$W0rD");
    UserDto? user = await _userService.ResetPasswordAsync(_user.EntityId, payload);
    Assert.NotNull(user);

    Assert.Equal(_user.EntityId, user.Id);
    Assert.Equal(2, user.Version);
    Assert.Equal(Actor, user.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(RealmDto, user.Realm);
    Assert.True(user.HasPassword);
    Assert.Equal(Actor, user.PasswordChangedBy);
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Equal(user.UpdatedOn.AsUniversalTime(), user.PasswordChangedOn.Value.AsUniversalTime());
  }

  [Fact(DisplayName = "It should save a user identifier.")]
  public async Task Given_UserFound_When_SaveIdentifier_Then_IdentifierSaved()
  {
    string key = "Google";
    SaveUserIdentifierPayload payload = new("  1234567890  ");
    UserDto? dto = await _userService.SaveIdentifierAsync(_user.EntityId, key, payload);
    Assert.NotNull(dto);

    Assert.Equal(_user.EntityId, dto.Id);
    Assert.Equal(2, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(1));

    CustomIdentifierDto customIdentifier = Assert.Single(dto.CustomIdentifiers);
    Assert.Equal(key, customIdentifier.Key);
    Assert.Equal(payload.Value.Trim(), customIdentifier.Value);
  }

  [Fact(DisplayName = "It should sign-out active user sessions.")]
  public async Task Given_Sessions_When_SignOut_Then_SignedOut()
  {
    Session session = new(_user);
    await _sessionRepository.SaveAsync(session);

    UserDto? dto = await _userService.SignOutAsync(_user.EntityId);
    Assert.NotNull(dto);
    Assert.Equal(_user.EntityId, dto.Id);
    Assert.Equal(_user.Version, dto.Version);
    Assert.Equal(_user.UpdatedOn.AsUniversalTime(), dto.UpdatedOn.AsUniversalTime());

    SessionEntity? entity = await KrakenarContext.Sessions.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == session.Id.Value);
    Assert.NotNull(entity);
    Assert.False(entity.IsActive);
    Assert.Equal(ActorId?.Value, entity.SignedOutBy);
    Assert.NotNull(entity.SignedOutOn);
    Assert.Equal(DateTime.UtcNow, entity.SignedOutOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should throw CustomIdentifierAlreadyUsedException when there is a custom identifier conflict.")]
  public async Task Given_CustomIdentifierConflict_When_SaveIdentifier_Then_CustomIdentifierAlreadyUsedException()
  {
    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    _user.SetCustomIdentifier(key, value, ActorId);

    User other = new(new UniqueName(new UniqueNameSettings(), Faker.Internet.UserName()), password: null, ActorId, UserId.NewId(Realm.Id));

    await _userRepository.SaveAsync([_user, other]);

    SaveUserIdentifierPayload payload = new(value.Value);
    var exception = await Assert.ThrowsAsync<CustomIdentifierAlreadyUsedException>(async () => await _userService.SaveIdentifierAsync(other.EntityId, key.Value, payload));

    Assert.Equal(Realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal("User", exception.EntityType);
    Assert.Equal(other.EntityId, exception.EntityId);
    Assert.Equal(_user.EntityId, exception.ConflictId);
    Assert.Equal(key.Value, exception.Key);
    Assert.Equal(value.Value, exception.Value);
  }

  [Fact(DisplayName = "It should throw EmailAddressAlreadyUsedException when an email address conflict occurs.")]
  public async Task Given_EmailAddressConflict_When_CreateOrReplace_Then_EmailAddressAlreadyUsedException()
  {
    _user.SetEmail(new Email(Faker.Person.Email), ActorId);
    await _userRepository.SaveAsync(_user);

    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = Faker.Internet.UserName(),
      Email = new EmailPayload(Faker.Person.Email, isVerified: true)
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await _userService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(id, exception.UserId);
    Assert.Equal([_user.EntityId], exception.ConflictIds);
    Assert.Equal(payload.Email.Address, exception.EmailAddress);
    Assert.Equal("Email", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple users were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    User user1 = new(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), password: null, ActorId, UserId.NewId(Realm.Id));

    User user2 = new(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), password: null, ActorId, UserId.NewId(Realm.Id));
    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    user2.SetCustomIdentifier(key, value, ActorId);

    await _userRepository.SaveAsync([user1, user2]);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserDto>>(
      async () => await _userService.ReadAsync(_user.EntityId, user1.UniqueName.Value, new CustomIdentifierDto(key.Value, value.Value)));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when a unique name conflict occurs.")]
  public async Task Given_UniqueNameConflict_When_CreateOrReplace_Then_UniqueNameAlreadyUsedException()
  {
    CreateOrReplaceUserPayload payload = new()
    {
      UniqueName = _user.UniqueName.Value
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _userService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(_user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("User", exception.EntityType);
    Assert.Equal(id, exception.EntityId);
    Assert.Equal(_user.EntityId, exception.ConflictId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when authenticating a user not found.")]
  public async Task Given_NotFound_When_Authenticate_Then_UserNotFoundException()
  {
    AuthenticateUserPayload payload = new("not_found", "no_password");
    AuthenticateUser command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _userService.AuthenticateAsync(payload));
    Assert.Equal(Realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal(payload.User, exception.User);
    Assert.Equal("User", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing user.")]
  public async Task Given_Exist_When_Update_Then_Updated()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    Role editor = new(new UniqueName(Realm.UniqueNameSettings, "editor"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync([admin, editor]);

    _user.Nickname = new PersonName("Oska");
    _user.SetUniqueName(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), ActorId);
    _user.AddRole(admin, ActorId);
    _user.AddRole(editor, ActorId);
    _user.SetCustomAttribute(new Identifier("EmployeeId"), Guid.NewGuid().ToString());
    string hin = Faker.Person.BuildHealthInsuranceNumber();
    _user.SetCustomAttribute(new Identifier("HealthInsuranceNumber"), hin);
    _user.SetCustomAttribute(new Identifier("IAL"), "1.0");
    _user.SetPhone(new Phone("+15148454636", "CA", "123456", isVerified: true), ActorId);
    _user.Update(ActorId);
    await _userRepository.SaveAsync(_user);
    long version = _user.Version;

    UpdateUserPayload payload = new()
    {
      UniqueName = Faker.Person.UserName,
      Password = new ChangePasswordPayload("Test123!", PasswordString),
      IsDisabled = true,
      Address = new Contracts.Change<AddressPayload>(new AddressPayload("150 Saint-Catherine St W", "Montreal", "CA", "H2X 3Y2", "QC")),
      Email = new Contracts.Change<EmailPayload>(new EmailPayload(Faker.Person.Email, isVerified: true)),
      Phone = new Contracts.Change<PhonePayload>(),
      FirstName = new Contracts.Change<string>(Faker.Person.FirstName),
      LastName = new Contracts.Change<string>(Faker.Person.LastName),
      Birthdate = new Contracts.Change<DateTime?>(Faker.Person.DateOfBirth),
      Gender = new Contracts.Change<string>(Faker.Person.Gender.ToString()),
      Locale = new Contracts.Change<string>(Faker.Locale),
      TimeZone = new Contracts.Change<string>("America/New_York"),
      Picture = new Contracts.Change<string>(Faker.Person.Avatar),
      Profile = new Contracts.Change<string>($"https://www.{Faker.Person.Website}/profile"),
      Website = new Contracts.Change<string>($"https://www.{Faker.Person.Website}")
    };
    payload.CustomAttributes.Add(new CustomAttribute("EmployeeId", string.Empty));
    payload.CustomAttributes.Add(new CustomAttribute("EmployeeNo", "110879"));
    payload.CustomAttributes.Add(new CustomAttribute("IAL", "2.0"));
    payload.Roles.Add(new RoleChange("  ADMIN  ", CollectionAction.Add));
    payload.Roles.Add(new RoleChange("editor", CollectionAction.Remove));

    UserDto? user = await _userService.UpdateAsync(_user.EntityId, payload);
    Assert.NotNull(user);
    Assert.Equal(_user.EntityId, user.Id);
    Assert.Equal(version + 8, user.Version);
    Assert.Equal(Actor, user.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.Equal(Actor, user.PasswordChangedBy);
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Equal(DateTime.UtcNow, user.PasswordChangedOn.Value, TimeSpan.FromSeconds(10));
    Assert.True(user.HasPassword);
    Assert.Equal(Actor, user.DisabledBy);
    Assert.NotNull(user.DisabledOn);
    Assert.Equal(DateTime.UtcNow, user.DisabledOn.Value, TimeSpan.FromSeconds(10));
    Assert.True(user.IsDisabled);

    Assert.NotNull(payload.Address.Value);
    Assert.NotNull(user.Address);
    Assert.Equal(payload.Address.Value.Street, user.Address.Street);
    Assert.Equal(payload.Address.Value.Locality, user.Address.Locality);
    Assert.Equal(payload.Address.Value.PostalCode, user.Address.PostalCode);
    Assert.Equal(payload.Address.Value.Region, user.Address.Region);
    Assert.Equal(payload.Address.Value.Country, user.Address.Country);
    Assert.Equal(new Address(user.Address).ToString(), user.Address.Formatted);
    Assert.False(user.Address.IsVerified);
    Assert.Null(user.Address.VerifiedBy);
    Assert.Null(user.Address.VerifiedOn);

    Assert.NotNull(payload.Email.Value);
    Assert.NotNull(user.Email);
    Assert.Equal(payload.Email.Value.Address, user.Email.Address);
    Assert.True(user.Email.IsVerified);
    Assert.Equal(Actor, user.Email.VerifiedBy);
    Assert.NotNull(user.Email.VerifiedOn);
    Assert.Equal(DateTime.UtcNow, user.Email.VerifiedOn.Value, TimeSpan.FromSeconds(10));

    Assert.Null(user.Phone);

    Assert.True(user.IsConfirmed);

    Assert.Equal(payload.FirstName.Value, user.FirstName);
    Assert.Null(user.MiddleName);
    Assert.Equal(payload.LastName.Value, user.LastName);
    Assert.Equal(_user.Nickname.Value, user.Nickname);
    Assert.NotNull(payload.Birthdate.Value);
    Assert.NotNull(user.Birthdate);
    Assert.Equal(payload.Birthdate.Value.Value.ToUniversalTime(), user.Birthdate.Value, TimeSpan.FromSeconds(10));
    Assert.Equal(payload.Gender.Value?.ToLowerInvariant(), user.Gender);
    Assert.Equal(payload.Locale.Value, user.Locale?.Code);
    Assert.Equal(payload.TimeZone.Value, user.TimeZone);
    Assert.Equal(payload.Picture.Value, user.Picture);
    Assert.Equal(payload.Profile.Value, user.Profile);
    Assert.Equal(payload.Website.Value, user.Website);

    Assert.Equal(3, user.CustomAttributes.Count);
    Assert.Contains(user.CustomAttributes, c => c.Key == "EmployeeNo" && c.Value == "110879");
    Assert.Contains(user.CustomAttributes, c => c.Key == "HealthInsuranceNumber" && c.Value == hin);
    Assert.Contains(user.CustomAttributes, c => c.Key == "IAL" && c.Value == "2.0");

    Assert.Single(user.Roles);
    Assert.Contains(user.Roles, r => r.Id == admin.EntityId);
  }
}
