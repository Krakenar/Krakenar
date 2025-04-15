using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Commands;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Users;

[Trait(Traits.Category, Categories.Integration)]
public class UserIntegrationTests : IntegrationTests
{
  private readonly ICommandHandler<AuthenticateUser, UserDto> _authenticateUser;
  private readonly ICommandHandler<ResetUserPassword, UserDto?> _resetUserPassword;

  public UserIntegrationTests() : base()
  {
    _authenticateUser = ServiceProvider.GetRequiredService<ICommandHandler<AuthenticateUser, UserDto>>();
    _resetUserPassword = ServiceProvider.GetRequiredService<ICommandHandler<ResetUserPassword, UserDto?>>();
  }

  [Fact(DisplayName = "Authenticate: it should throw UserNotFoundException when the user was not found.")]
  public async Task Given_NotFound_When_Authenticate_Then_UserNotFoundException()
  {
    AuthenticateUserPayload payload = new("not_found", "no_password");
    AuthenticateUser command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _authenticateUser.HandleAsync(command));
    Assert.Null(exception.RealmId);
    Assert.Equal(payload.User, exception.User);
    Assert.Equal("User", exception.PropertyName);
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

  [Fact(DisplayName = "It should reset the user password.")]
  public async Task Given_UserFound_When_ResetPassword_Then_PasswordReset()
  {
    Guid id = await KrakenarContext.Users.AsNoTracking().Select(x => x.Id).SingleOrDefaultAsync();
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
}
