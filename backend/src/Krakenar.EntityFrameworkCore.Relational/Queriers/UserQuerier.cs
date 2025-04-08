using Krakenar.Core;
using Krakenar.Core.Users;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class UserQuerier : IUserQuerier // TODO(fpion): implement
{
  public virtual Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
