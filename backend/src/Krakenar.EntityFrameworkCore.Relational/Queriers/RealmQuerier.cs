using Krakenar.Core;
using Krakenar.Core.Realms;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class RealmQuerier : IRealmQuerier // TODO(fpion): implement
{
  public Task<RealmId?> FindIdAsync(Slug uniqueSlug, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<RealmDto> ReadAsync(Realm realm, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public Task<RealmDto?> ReadAsync(RealmId id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public Task<RealmDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public Task<RealmDto?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
