using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using User = Krakenar.Core.Users.User;
using UserDto = Krakenar.Contracts.Users.User;

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

  public virtual Task<UserDto> ReadAsync(User user, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(UserId id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<UserDto?> ReadAsync(CustomIdentifierDto identifier, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<IReadOnlyCollection<UserDto>> ReadAsync(IEmail email, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
