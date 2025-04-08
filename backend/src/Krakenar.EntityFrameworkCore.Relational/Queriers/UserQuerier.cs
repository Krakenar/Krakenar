using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class UserQuerier : IUserQuerier // TODO(fpion): implement
{
  public virtual Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserId?> FindIdAsync(Identifier key, CustomIdentifier value, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<IReadOnlyCollection<UserId>> FindIdsAsync(IEmail email, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
