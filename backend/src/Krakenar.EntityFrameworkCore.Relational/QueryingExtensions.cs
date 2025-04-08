using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.Entities;

namespace Krakenar.EntityFrameworkCore.Relational;

public static class QueryingExtensions
{
  public static IQueryable<T> WhereRealm<T>(this IQueryable<T> query, RealmId? realmId) where T : ISegregatedEntity
  {
    return realmId.HasValue ? query.Where(x => x.RealmUid == realmId.Value.ToGuid()) : query.Where(x => x.RealmUid == null);
  }
}
