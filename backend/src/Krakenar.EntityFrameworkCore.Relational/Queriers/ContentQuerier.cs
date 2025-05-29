using Krakenar.Contracts.Actors;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Content = Krakenar.Core.Contents.Content;
using ContentDto = Krakenar.Contracts.Contents.Content;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class ContentQuerier : IContentQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Content> Contents { get; }
  protected virtual ISqlHelper SqlHelper { get; }
  protected virtual DbSet<Entities.UniqueIndex> UniqueIndex { get; }

  public ContentQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Contents = context.Contents;
    SqlHelper = sqlHelper;
    UniqueIndex = context.UniqueIndex;
  }

  public virtual async Task<IReadOnlyDictionary<Guid, ContentId>> FindConflictsAsync(
    ContentTypeId contentTypeId,
    LanguageId? languageId,
    ContentStatus status,
    IReadOnlyDictionary<Guid, FieldValue> fieldValues,
    ContentId contentId,
    CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;
    Guid contentTypeUid = contentTypeId.EntityId;
    Guid? languageUid = languageId?.EntityId;
    HashSet<string> keys = [.. fieldValues.Select(Entities.UniqueIndex.CreateKey)];
    Guid contentUid = contentId.EntityId;

    var conflicts = await UniqueIndex.AsNoTracking()
      .WhereRealm(realmId)
      .Where(x => x.ContentTypeUid == contentTypeUid
        && (languageUid.HasValue ? x.LanguageUid == languageUid.Value : x.LanguageUid == null)
        && x.Status == status
        && keys.Contains(x.Key)
        && x.ContentUid != contentUid)
      .Select(x => new
      {
        FieldDefinitionId = x.FieldDefinitionUid,
        ContentId = x.ContentUid
      })
      .ToArrayAsync(cancellationToken);

    return conflicts.ToDictionary(x => x.FieldDefinitionId, x => new ContentId(x.ContentId, realmId));
  }

  public virtual async Task<IReadOnlyDictionary<Guid, Guid>> FindContentTypeIdsAsync(IEnumerable<Guid> contentIds, CancellationToken cancellationToken)
  {
    HashSet<Guid> distinctIds = [.. contentIds];

    var associations = await Contents.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.ContentType)
      .Where(x => distinctIds.Contains(x.Id))
      .Select(x => new
      {
        ContentId = x.Id,
        ContentTypeId = x.ContentType!.Id
      })
      .ToArrayAsync(cancellationToken);

    return associations.ToDictionary(x => x.ContentId, x => x.ContentTypeId);
  }

  public virtual async Task<ContentId?> FindIdAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await Contents
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.ContentType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .Where(x => x.ContentType!.StreamId == contentTypeId.Value
        && x.Locales.Any(l => (languageId.HasValue ? (l.Language!.StreamId == languageId.Value.Value) : (l.LanguageId == null))
          && l.UniqueNameNormalized == uniqueNameNormalized))
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new ContentId(streamId);
  }

  public virtual async Task<IReadOnlyCollection<ContentId>> FindIdsAsync(ContentTypeId contentTypeId, CancellationToken cancellationToken)
  {
    string[] streamIds = await Contents
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.ContentType)
      .Where(x => x.ContentType!.StreamId == contentTypeId.Value)
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new ContentId(streamId)).ToList().AsReadOnly();
  }
  public virtual async Task<IReadOnlyCollection<ContentId>> FindIdsAsync(LanguageId languageId, CancellationToken cancellationToken)
  {
    string[] streamIds = await Contents
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .Where(x => x.Locales.Any(l => l.Language!.StreamId == languageId.Value))
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new ContentId(streamId)).ToList().AsReadOnly();
  }

  public virtual async Task<ContentDto> ReadAsync(Content content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken) ?? throw new InvalidOperationException($"The content entity 'StreamId={content.Id}' could not be found.");
  }
  public virtual async Task<ContentDto?> ReadAsync(ContentId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<ContentDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Content? content = await Contents.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.ContentType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return content is null ? null : await MapAsync(content, cancellationToken);
  }

  protected virtual async Task<ContentDto> MapAsync(Entities.Content content, CancellationToken cancellationToken)
  {
    return (await MapAsync([content], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<ContentDto>> MapAsync(IEnumerable<Entities.Content> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    RealmDto? realm = ApplicationContext.Realm;
    return contents.Select(content => mapper.ToContent(content, realm)).ToList().AsReadOnly();
  }
}
