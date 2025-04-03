using Krakenar.Core.Users.Events;
using MediatR;

namespace Krakenar.Core.Users.Commands;

public record SaveUserCommand(User User) : IRequest;

internal class SaveUserCommandHandler : IRequestHandler<SaveUserCommand>
{
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SaveUserCommandHandler(IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task Handle(SaveUserCommand command, CancellationToken cancellationToken)
  {
    User user = command.User;

    bool hasUniqueNameChanged = user.Changes.Any(change => change is UserCreated);
    if (hasUniqueNameChanged)
    {
      UserId? conflictId = await _userQuerier.FindIdAsync(user.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(user.Id))
      {
        throw new UniqueNameAlreadyUsedException(user, conflictId.Value);
      }
    }

    await _userRepository.SaveAsync(user, cancellationToken);
  }
}
