using FluentValidation;
using Krakenar.Contracts.Users;
using Krakenar.Core.Validators;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record SaveUserIdentifier(Guid Id, string Key, SaveUserIdentifierPayload Payload) : ICommand<UserDto?>;

/// <exception cref="CustomIdentifierAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class SaveUserIdentifierHandler : ICommandHandler<SaveUserIdentifier, UserDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }
  protected virtual IUserService UserService { get; }

  public SaveUserIdentifierHandler(
    IApplicationContext applicationContext,
    IUserQuerier userQuerier,
    IUserRepository userRepository,
    IUserService userService)
  {
    ApplicationContext = applicationContext;
    UserQuerier = userQuerier;
    UserRepository = userRepository;
    UserService = userService;
  }

  public virtual async Task<UserDto?> HandleAsync(SaveUserIdentifier command, CancellationToken cancellationToken)
  {
    SaveUserIdentifierPayload payload = command.Payload;
    CustomIdentifierDto customIdentifier = new(command.Key, payload.Value);
    new CustomIdentifierValidator().ValidateAndThrow(customIdentifier);

    UserId userId = new(command.Id, ApplicationContext.RealmId);
    User? user = await UserRepository.LoadAsync(userId, cancellationToken);
    if (user is null)
    {
      return null;
    }

    Identifier key = new(customIdentifier.Key);
    CustomIdentifier value = new(customIdentifier.Value);
    user.SetCustomIdentifier(key, value, ApplicationContext.ActorId);

    await UserService.SaveAsync(user, cancellationToken);

    return await UserQuerier.ReadAsync(user, cancellationToken);
  }
}
