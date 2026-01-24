using FluentValidation;
using Krakenar.Contracts.Users;
using Krakenar.Core.Authentication;
using Krakenar.Core.Logging;
using Krakenar.Core.Users.Validators;
using Logitar;
using Logitar.CQRS;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record AuthenticateUser(AuthenticateUserPayload Payload) : IAnonymizable, ICommand<UserDto>
{
  public object? Anonymize()
  {
    AuthenticateUser clone = this.DeepClone();
    clone.Payload.Password = Payload.Password.Mask();
    return clone;
  }
}

/// <exception cref="IncorrectUserPasswordException"></exception>
/// <exception cref="UserHasNoPasswordException"></exception>
/// <exception cref="UserIsDisabledException"></exception>
/// <exception cref="UserNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class AuthenticateUserHandler : ICommandHandler<AuthenticateUser, UserDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IAuthenticationService AuthenticationService { get; }
  protected virtual IUserManager UserManager { get; }
  protected virtual IUserQuerier UserQuerier { get; }

  public AuthenticateUserHandler(
    IApplicationContext applicationContext,
    IAuthenticationService authenticationService,
    IUserManager userManager,
    IUserQuerier userQuerier)
  {
    ApplicationContext = applicationContext;
    AuthenticationService = authenticationService;
    UserManager = userManager;
    UserQuerier = userQuerier;
  }

  public virtual async Task<UserDto> HandleAsync(AuthenticateUser command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    FoundUsers users = await UserManager.FindAsync(payload.User, cancellationToken);
    User user = users.ById ?? users.ByUniqueName ?? users.ByEmailAddress ?? users.ByCustomIdentifier
      ?? throw new UserNotFoundException(ApplicationContext.RealmId, payload.User, nameof(payload.User));

    user.Authenticate(payload.Password, ApplicationContext.ActorId);

    await AuthenticationService.AuthenticatedAsync(user, cancellationToken);

    return await UserQuerier.ReadAsync(user, cancellationToken);
  }
}
