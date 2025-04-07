namespace Krakenar.Core.Users;

public interface IUserQuerier // TODO(fpion): implement
{
  Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);
}
