using Krakenar.Contracts.Users;

namespace Krakenar.Core.Users;

public interface IUserQuerier
{
  Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);
  Task<UserId?> FindIdAsync(Identifier key, CustomIdentifier value, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<UserId>> FindIdsAsync(IEmail email, CancellationToken cancellationToken = default);
}
