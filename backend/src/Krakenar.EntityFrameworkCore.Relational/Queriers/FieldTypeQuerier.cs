using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Fields;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using FieldType = Krakenar.Core.Fields.FieldType;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class FieldTypeQuerier : IFieldTypeQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.FieldType> FieldTypes { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public FieldTypeQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    FieldTypes = context.FieldTypes;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await FieldTypes.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new FieldTypeId(streamId);
  }

  public virtual async Task<FieldTypeDto> ReadAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    return await ReadAsync(fieldType.Id, cancellationToken) ?? throw new InvalidOperationException($"The field type entity 'StreamId={fieldType.Id}' could not be found.");
  }
  public virtual async Task<FieldTypeDto?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<FieldTypeDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.FieldType? fieldType = await FieldTypes.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return fieldType is null ? null : await MapAsync(fieldType, cancellationToken);
  }
  public virtual async Task<FieldTypeDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    Entities.FieldType? fieldType = await FieldTypes.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return fieldType is null ? null : await MapAsync(fieldType, cancellationToken);
  }

  public virtual async Task<SearchResults<FieldTypeDto>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.FieldTypes.Table).SelectAll(KrakenarDb.FieldTypes.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.FieldTypes.RealmUid)
      .ApplyIdFilter(KrakenarDb.FieldTypes.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.FieldTypes.UniqueName, KrakenarDb.FieldTypes.DisplayName);

    if (payload.DataType.HasValue)
    {
      builder.Where(KrakenarDb.FieldTypes.DataType, Operators.IsEqualTo(payload.DataType.Value.ToString()));
    }

    IQueryable<Entities.FieldType> query = FieldTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.FieldType>? ordered = null;
    foreach (FieldTypeSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case FieldTypeSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case FieldTypeSort.DisplayName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case FieldTypeSort.UniqueName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case FieldTypeSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.FieldType[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<FieldTypeDto> fieldTypes = await MapAsync(entities, cancellationToken);

    return new SearchResults<FieldTypeDto>(fieldTypes, total);
  }

  protected virtual async Task<FieldTypeDto> MapAsync(Entities.FieldType fieldType, CancellationToken cancellationToken)
  {
    return (await MapAsync([fieldType], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<FieldTypeDto>> MapAsync(IEnumerable<Entities.FieldType> fieldTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = fieldTypes.SelectMany(fieldType => fieldType.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return fieldTypes.Select(fieldType => mapper.ToFieldType(fieldType, realm)).ToList().AsReadOnly();
  }
}
