namespace Krakenar.Core.Users;

public interface IUserRepository
{
  Task SaveAsync(User user, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
}
