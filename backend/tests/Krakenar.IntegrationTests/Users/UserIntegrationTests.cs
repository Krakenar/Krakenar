using Bogus;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Passwords;
using Krakenar.Core.Sessions;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Commands;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using Email = Krakenar.Core.Users.Email;
using SessionEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Session;
using User = Krakenar.Core.Users.User;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Users;

[Trait(Traits.Category, Categories.Integration)]
public class UserIntegrationTests : IntegrationTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly IPasswordService _passwordService;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;
  private readonly IUserService _userService;

  private readonly User _user;

  public UserIntegrationTests() : base()
  {
    _passwordService = ServiceProvider.GetRequiredService<IPasswordService>();
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
}
