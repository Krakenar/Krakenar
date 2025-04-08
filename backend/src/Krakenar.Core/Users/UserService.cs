using Krakenar.Core.Users.Events;

namespace Krakenar.Core.Users;

public interface IUserService
{
  Task SaveAsync(User user, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public UserService(IUserQuerier userQuerier, IUserRepository userRepository)
  {
    UserQuerier = userQuerier;
    UserRepository = userRepository;
  }

  public virtual async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = user.Changes.Any(change => change is UserCreated || change is UserUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      UserId? conflictId = await UserQuerier.FindIdAsync(user.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(user.Id))
      {
        throw new UniqueNameAlreadyUsedException(user, conflictId.Value);
      }
    }

    await UserRepository.SaveAsync(user, cancellationToken);
  }
}
