namespace Krakenar.Core.Users;

public interface IUserQuerier
{
  Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);
}
