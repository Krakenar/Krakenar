using Bogus;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Commands;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using User = Krakenar.Core.Users.User;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Users;

[Trait(Traits.Category, Categories.Integration)]
public class UserIntegrationTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  private readonly ICommandHandler<AuthenticateUser, UserDto> _authenticateUser;
  private readonly ICommandHandler<RemoveUserIdentifier, UserDto?> _removeUserIdentifier;
  private readonly ICommandHandler<ResetUserPassword, UserDto?> _resetUserPassword;
  private readonly ICommandHandler<SaveUserIdentifier, UserDto?> _saveUserIdentifier;

  public UserIntegrationTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    _authenticateUser = ServiceProvider.GetRequiredService<ICommandHandler<AuthenticateUser, UserDto>>();
    _removeUserIdentifier = ServiceProvider.GetRequiredService<ICommandHandler<RemoveUserIdentifier, UserDto?>>();
    _resetUserPassword = ServiceProvider.GetRequiredService<ICommandHandler<ResetUserPassword, UserDto?>>();
    _saveUserIdentifier = ServiceProvider.GetRequiredService<ICommandHandler<SaveUserIdentifier, UserDto?>>();
  }

  [Fact(DisplayName = "It should authenticate the user.")]
  public async Task Given_Valid_When_Authenticate_Then_Authenticated()
  {
    AuthenticateUserPayload payload = new("admin", "P@s$W0rD");
    AuthenticateUser command = new(payload);
    UserDto user = await _authenticateUser.HandleAsync(command);

    Assert.Null(user.Realm);
    Assert.Equal(payload.User, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should remove a user identifier.")]
  public async Task Given_UserFound_When_RemoveIdentifier_Then_IdentifierRemoved()
  {
    string? streamId = await KrakenarContext.Users.AsNoTracking().Select(x => x.StreamId).SingleOrDefaultAsync();
    Assert.NotNull(streamId);

    UserId userId = new(streamId);
    User? user = await _userRepository.LoadAsync(userId);
    Assert.NotNull(user);

    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    user.SetCustomIdentifier(key, value, ActorId);
    await _userRepository.SaveAsync(user);

    RemoveUserIdentifier command = new(user.EntityId, key.Value);
    UserDto? dto = await _removeUserIdentifier.HandleAsync(command);
    Assert.NotNull(dto);

    Assert.Equal(command.Id, dto.Id);
    Assert.Equal(user.Version + 1, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(1));
    Assert.Empty(dto.CustomIdentifiers);
  }

  [Fact(DisplayName = "It should save a user identifier.")]
  public async Task Given_UserFound_When_SaveIdentifier_Then_IdentifierSaved()
  {
    Guid id = await KrakenarContext.Users.AsNoTracking().Select(x => x.Id).SingleAsync();

    SaveUserIdentifierPayload payload = new("  1234567890  ");
    SaveUserIdentifier command = new(id, "Google", payload);
    UserDto? dto = await _saveUserIdentifier.HandleAsync(command);
    Assert.NotNull(dto);

    Assert.Equal(command.Id, dto.Id);
    Assert.Equal(2, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(1));

    CustomIdentifierDto customIdentifier = Assert.Single(dto.CustomIdentifiers);
    Assert.Equal(command.Key, customIdentifier.Key);
    Assert.Equal(payload.Value.Trim(), customIdentifier.Value);
  }

  [Fact(DisplayName = "It should reset the user password.")]
  public async Task Given_UserFound_When_ResetPassword_Then_PasswordReset()
  {
    Guid id = await KrakenarContext.Users.AsNoTracking().Select(x => x.Id).SingleAsync();
    ResetUserPasswordPayload payload = new("N3wP@s$W0rD");
    ResetUserPassword command = new(id, payload);

    UserDto? user = await _resetUserPassword.HandleAsync(command);
    Assert.NotNull(user);

    Assert.Equal(id, user.Id);
    Assert.Equal(2, user.Version);
    Assert.Equal(Actor, user.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, user.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Null(user.Realm);
    Assert.True(user.HasPassword);
    Assert.Equal(Actor, user.PasswordChangedBy);
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Equal(user.UpdatedOn.AsUniversalTime(), user.PasswordChangedOn.Value.AsUniversalTime());
  }

  [Fact(DisplayName = "It should throw CustomIdentifierAlreadyUsedException when there is a custom identifier conflict.")]
  public async Task Given_CustomIdentifierConflict_When_SaveIdentifier_Then_CustomIdentifierAlreadyUsedException()
  {
    string? streamId = await KrakenarContext.Users.AsNoTracking().Select(x => x.StreamId).SingleOrDefaultAsync();
    Assert.NotNull(streamId);

    UserId userId = new(streamId);
    User? user = await _userRepository.LoadAsync(userId);
    Assert.NotNull(user);

    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    user.SetCustomIdentifier(key, value, ActorId);

    User other = new(new UniqueName(new UniqueNameSettings(), Faker.Internet.UserName()), password: null, ActorId);

    await _userRepository.SaveAsync([user, other]);

    SaveUserIdentifierPayload payload = new(value.Value);
    SaveUserIdentifier command = new(other.EntityId, key.Value, payload);
    var exception = await Assert.ThrowsAsync<CustomIdentifierAlreadyUsedException>(async () => await _saveUserIdentifier.HandleAsync(command));

    Assert.Null(exception.RealmId);
    Assert.Equal("User", exception.EntityType);
    Assert.Equal(other.EntityId, exception.EntityId);
    Assert.Equal(user.EntityId, exception.ConflictId);
    Assert.Equal(key.Value, exception.Key);
    Assert.Equal(value.Value, exception.Value);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when authenticating a user not found.")]
  public async Task Given_NotFound_When_Authenticate_Then_UserNotFoundException()
  {
    AuthenticateUserPayload payload = new("not_found", "no_password");
    AuthenticateUser command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _authenticateUser.HandleAsync(command));
    Assert.Null(exception.RealmId);
    Assert.Equal(payload.User, exception.User);
    Assert.Equal("User", exception.PropertyName);
  }
}
