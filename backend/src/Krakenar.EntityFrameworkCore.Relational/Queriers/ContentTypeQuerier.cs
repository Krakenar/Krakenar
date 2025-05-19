using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ContentType = Krakenar.Core.Contents.ContentType;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class ContentTypeQuerier : IContentTypeQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.ContentType> ContentTypes { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public ContentTypeQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    ContentTypes = context.ContentTypes;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<ContentTypeId?> FindIdAsync(Identifier uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await ContentTypes.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new ContentTypeId(streamId);
  }

  public virtual async Task<IReadOnlyCollection<ContentTypeId>> FindIdsAsync(FieldTypeId fieldTypeId, CancellationToken cancellationToken)
  {
    string[] streamIds = await ContentTypes.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.FieldDefinitions).ThenInclude(x => x.FieldType)
      .Where(x => x.FieldDefinitions.Any(f => f.FieldType!.StreamId == fieldTypeId.Value))
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new ContentTypeId(streamId)).ToList().AsReadOnly();
  }

  public virtual async Task<ContentTypeDto> ReadAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    return await ReadAsync(contentType.Id, cancellationToken) ?? throw new InvalidOperationException($"The content type entity 'StreamId={contentType.Id}' could not be found.");
  }
  public virtual async Task<ContentTypeDto?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<ContentTypeDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.ContentType? contentType = await ContentTypes.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.FieldDefinitions).ThenInclude(x => x.FieldType)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return contentType is null ? null : await MapAsync(contentType, cancellationToken);
  }
  public virtual async Task<ContentTypeDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    Entities.ContentType? contentType = await ContentTypes.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.FieldDefinitions).ThenInclude(x => x.FieldType)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return contentType is null ? null : await MapAsync(contentType, cancellationToken);
  }

  public virtual async Task<SearchResults<ContentTypeDto>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.ContentTypes.Table).SelectAll(KrakenarDb.ContentTypes.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.ContentTypes.RealmUid)
      .ApplyIdFilter(KrakenarDb.ContentTypes.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.ContentTypes.UniqueName, KrakenarDb.ContentTypes.DisplayName);

    if (payload.IsInvariant.HasValue)
    {
      builder.Where(KrakenarDb.ContentTypes.IsInvariant, Operators.IsEqualTo(payload.IsInvariant.Value));
    }

    IQueryable<Entities.ContentType> query = ContentTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.ContentType>? ordered = null;
    foreach (ContentTypeSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ContentTypeSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ContentTypeSort.DisplayName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ContentTypeSort.UniqueName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case ContentTypeSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.ContentType[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ContentTypeDto> contentTypes = await MapAsync(entities, cancellationToken);

    return new SearchResults<ContentTypeDto>(contentTypes, total);
  }

  protected virtual async Task<ContentTypeDto> MapAsync(Entities.ContentType contentType, CancellationToken cancellationToken)
  {
    return (await MapAsync([contentType], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<ContentTypeDto>> MapAsync(IEnumerable<Entities.ContentType> contentTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contentTypes.SelectMany(contentType => contentType.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return contentTypes.Select(contentType => mapper.ToContentType(contentType, realm)).ToList().AsReadOnly();
  }
}
