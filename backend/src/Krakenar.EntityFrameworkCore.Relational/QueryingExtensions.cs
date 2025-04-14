using Krakenar.Contracts.Search;
using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational;

public static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder builder, ColumnId column, IEnumerable<Guid> ids)
  {
    if (!ids.Any())
    {
      return builder;
    }

    object[] values = ids.Distinct().Select(id => (object)id).ToArray();
    return builder.Where(column, Operators.IsIn(values));
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    return query.ApplyPaging(payload.Skip, payload.Limit);
  }
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int limit)
  {
    if (skip > 0)
    {
      query = query.Skip(skip);
    }
    if (limit > 0)
    {
      query = query.Take(limit);
    }

    return query;
  }

  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQueryBuilder query) where T : class
  {
    return entities.FromQuery(query.Build());
  }
  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQuery query) where T : class
  {
    return entities.FromSqlRaw(query.Text, query.Parameters.ToArray());
  }

  public static IQueryable<T> WhereRealm<T>(this IQueryable<T> query, RealmId? realmId) where T : ISegregatedEntity
  {
    return realmId.HasValue ? query.Where(x => x.RealmUid == realmId.Value.ToGuid()) : query.Where(x => x.RealmUid == null);
  }
  public static IQueryBuilder WhereRealm(this IQueryBuilder query, RealmId? realmId, ColumnId column)
  {
    return query.Where(column, realmId.HasValue ? Operators.IsEqualTo(realmId.Value.ToGuid()) : Operators.IsNull());
  }
}
