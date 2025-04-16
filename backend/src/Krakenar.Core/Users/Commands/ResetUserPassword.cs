using FluentValidation;
using Krakenar.Contracts.Users;
using Krakenar.Core.Passwords;
using Krakenar.Core.Users.Validators;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record ResetUserPassword(Guid Id, ResetUserPasswordPayload Payload) : ICommand<UserDto?>;

public class ResetUserPasswordHandler : ICommandHandler<ResetUserPassword, UserDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IPasswordService PasswordService { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public ResetUserPasswordHandler(
    IApplicationContext applicationContext,
    IPasswordService passwordService,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    ApplicationContext = applicationContext;
    PasswordService = passwordService;
    UserQuerier = userQuerier;
    UserRepository = userRepository;
  }

  public virtual async Task<UserDto?> HandleAsync(ResetUserPassword command, CancellationToken cancellationToken)
  {
    ResetUserPasswordPayload payload = command.Payload;
    new ResetUserPasswordValidator(ApplicationContext.PasswordSettings).ValidateAndThrow(payload);

    UserId userId = new(command.Id, ApplicationContext.RealmId);
    User? user = await UserRepository.LoadAsync(userId, cancellationToken);
    if (user is null)
    {
      return null;
    }

    Password password = PasswordService.ValidateAndHash(payload.Password);
    user.ResetPassword(password, ApplicationContext.ActorId);

    await UserRepository.SaveAsync(user, cancellationToken);

    return await UserQuerier.ReadAsync(user, cancellationToken);
  }
}
