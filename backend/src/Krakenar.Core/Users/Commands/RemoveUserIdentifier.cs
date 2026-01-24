using FluentValidation;
using Krakenar.Core.Validators;
using Logitar.CQRS;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record RemoveUserIdentifier(Guid Id, string Key) : ICommand<UserDto?>;

/// <exception cref="ValidationException"></exception>
public class RemoveUserIdentifierHandler : ICommandHandler<RemoveUserIdentifier, UserDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public RemoveUserIdentifierHandler(IApplicationContext applicationContext, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    ApplicationContext = applicationContext;
    UserQuerier = userQuerier;
    UserRepository = userRepository;
  }

  public virtual async Task<UserDto?> HandleAsync(RemoveUserIdentifier command, CancellationToken cancellationToken)
  {
    CustomIdentifierDto customIdentifier = new(command.Key, value: "Temporary");
    new CustomIdentifierValidator().ValidateAndThrow(customIdentifier);

    UserId userId = new(command.Id, ApplicationContext.RealmId);
    User? user = await UserRepository.LoadAsync(userId, cancellationToken);
    if (user is null)
    {
      return null;
    }

    Identifier key = new(customIdentifier.Key);
    user.RemoveCustomIdentifier(key, ApplicationContext.ActorId);

    await UserRepository.SaveAsync(user, cancellationToken);

    return await UserQuerier.ReadAsync(user, cancellationToken);
  }
}
