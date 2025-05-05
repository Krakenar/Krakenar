using Krakenar.Contracts.Actors;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.ApiKeys;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ApiKey = Krakenar.Core.ApiKeys.ApiKey;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class ApiKeyQuerier : IApiKeyQuerier
{
  protected virtual DbSet<Entities.ApiKey> ApiKeys { get; }
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public ApiKeyQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApiKeys = context.ApiKeys;
    ApplicationContext = applicationContext;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<ApiKeyDto> ReadAsync(ApiKey apiKey, CancellationToken cancellationToken)
  {
    return await ReadAsync(apiKey.Id, cancellationToken) ?? throw new InvalidOperationException($"The API key entity 'StreamId={apiKey.Id}' could not be found.");
  }
  public virtual async Task<ApiKeyDto?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<ApiKeyDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.ApiKey? apiKey = await ApiKeys.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return apiKey is null ? null : await MapAsync(apiKey, cancellationToken);
  }

  public virtual async Task<SearchResults<ApiKeyDto>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.ApiKeys.Table).SelectAll(KrakenarDb.ApiKeys.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.ApiKeys.RealmUid)
      .ApplyIdFilter(KrakenarDb.ApiKeys.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.ApiKeys.Name);

    if (payload.HasAuthenticated.HasValue)
    {
      NullOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(KrakenarDb.ApiKeys.AuthenticatedOn, @operator);
    }
    if (payload.RoleId.HasValue)
    {
      builder.Join(ApiKeyRoles.ApiKeyId, KrakenarDb.ApiKeys.ApiKeyId)
        .Join(Roles.RoleId, ApiKeyRoles.RoleId)
        .Where(Roles.Id, Operators.IsEqualTo(payload.RoleId.Value));
    }
    if (payload.Status != null)
    {
      DateTime moment = payload.Status.Moment?.ToUniversalTime() ?? DateTime.UtcNow;
      builder.Where(KrakenarDb.ApiKeys.ExpiresOn, payload.Status.IsExpired ? Operators.IsLessThanOrEqualTo(moment) : Operators.IsGreaterThan(moment));
    }

    IQueryable<Entities.ApiKey> query = ApiKeys.FromQuery(builder).AsNoTracking()
      .Include(x => x.Roles);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.ApiKey>? ordered = null;
    foreach (ApiKeySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ApiKeySort.AuthenticatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.AuthenticatedOn) : query.OrderBy(x => x.AuthenticatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.AuthenticatedOn) : ordered.ThenBy(x => x.AuthenticatedOn));
          break;
        case ApiKeySort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ApiKeySort.ExpiresOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.ExpiresOn) : query.OrderBy(x => x.ExpiresOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.ExpiresOn) : ordered.ThenBy(x => x.ExpiresOn));
          break;
        case ApiKeySort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case ApiKeySort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.ApiKey[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ApiKeyDto> apiKeys = await MapAsync(entities, cancellationToken);

    return new SearchResults<ApiKeyDto>(apiKeys, total);
  }

  protected virtual async Task<ApiKeyDto> MapAsync(Entities.ApiKey apiKey, CancellationToken cancellationToken)
  {
    return (await MapAsync([apiKey], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<ApiKeyDto>> MapAsync(IEnumerable<Entities.ApiKey> apiKeys, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(apiKey => apiKey.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, realm)).ToList().AsReadOnly();
  }
}
