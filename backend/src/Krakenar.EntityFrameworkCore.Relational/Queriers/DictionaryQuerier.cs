using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Localization;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Dictionary = Krakenar.Core.Dictionaries.Dictionary;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class DictionaryQuerier : IDictionaryQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Dictionary> Dictionaries { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public DictionaryQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Dictionaries = context.Dictionaries;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<DictionaryId?> FindIdAsync(LanguageId languageId, CancellationToken cancellationToken)
  {
    string? streamId = await Dictionaries.AsNoTracking()
      .Include(x => x.Language)
      .Where(x => x.Language!.StreamId == languageId.Value)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new DictionaryId(streamId);
  }

  public virtual async Task<DictionaryDto> ReadAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    return await ReadAsync(dictionary.Id, cancellationToken) ?? throw new InvalidOperationException($"The dictionary entity 'StreamId={dictionary.Id}' could not be found.");
  }
  public virtual async Task<DictionaryDto?> ReadAsync(DictionaryId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<DictionaryDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Dictionary? dictionary = await Dictionaries.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Entries)
      .Include(x => x.Language)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return dictionary is null ? null : await MapAsync(dictionary, cancellationToken);
  }
  public virtual async Task<DictionaryDto?> ReadByLanguageAsync(Guid languageId, CancellationToken cancellationToken)
  {
    Entities.Dictionary? dictionary = await Dictionaries.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.Entries)
      .Include(x => x.Language)
      .SingleOrDefaultAsync(x => x.Language!.Id == languageId, cancellationToken);

    return dictionary is null ? null : await MapAsync(dictionary, cancellationToken);
  }

  public virtual async Task<SearchResults<DictionaryDto>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Dictionaries.Table).SelectAll(KrakenarDb.Dictionaries.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Dictionaries.RealmUid)
      .ApplyIdFilter(KrakenarDb.Dictionaries.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search); // TODO(fpion): search into Language

    IQueryable<Entities.Dictionary> query = Dictionaries.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Dictionary>? ordered = null;
    foreach (DictionarySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case DictionarySort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case DictionarySort.EntryCount:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EntryCount) : query.OrderBy(x => x.EntryCount))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EntryCount) : ordered.ThenBy(x => x.EntryCount));
          break;
        case DictionarySort.Language:
          //ordered = (ordered is null)
          //  ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
          //  : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName)); // TODO(fpion): implement
          break;
        case DictionarySort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Dictionary[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<DictionaryDto> dictionaries = await MapAsync(entities, cancellationToken);

    return new SearchResults<DictionaryDto>(dictionaries, total);
  }

  protected virtual async Task<DictionaryDto> MapAsync(Entities.Dictionary dictionary, CancellationToken cancellationToken)
  {
    return (await MapAsync([dictionary], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<DictionaryDto>> MapAsync(IEnumerable<Entities.Dictionary> dictionaries, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = dictionaries.SelectMany(dictionary => dictionary.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return dictionaries.Select(dictionary => mapper.ToDictionary(dictionary, realm)).ToList().AsReadOnly();
  }
}
