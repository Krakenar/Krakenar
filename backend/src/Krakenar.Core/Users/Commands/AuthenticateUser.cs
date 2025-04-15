using FluentValidation;
using Krakenar.Contracts.Users;
using Krakenar.Core.Users.Validators;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record AuthenticateUser(AuthenticateUserPayload Payload) : ICommand<UserDto>;

/// <exception cref="IncorrectUserPasswordException"></exception>
/// <exception cref="UserHasNoPasswordException"></exception>
/// <exception cref="UserIsDisabledException"></exception>
/// <exception cref="UserNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class AuthenticateUserHandler : ICommandHandler<AuthenticateUser, UserDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserService UserService { get; }

  public AuthenticateUserHandler(IApplicationContext applicationContext, IUserQuerier userQuerier, IUserService userService)
  {
    ApplicationContext = applicationContext;
    UserQuerier = userQuerier;
    UserService = userService;
  }

  public virtual async Task<UserDto> HandleAsync(AuthenticateUser command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    FoundUsers users = await UserService.FindAsync(payload.User, cancellationToken);
    User user = users.ById ?? users.ByUniqueName ?? users.ByEmailAddress ?? users.ByCustomIdentifier
      ?? throw new UserNotFoundException(ApplicationContext.RealmId, payload.User, nameof(payload.User));

    user.Authenticate(payload.Password, ApplicationContext.ActorId);

    await UserService.SaveAsync(user, cancellationToken);

    return await UserQuerier.ReadAsync(user, cancellationToken);
  }
}
