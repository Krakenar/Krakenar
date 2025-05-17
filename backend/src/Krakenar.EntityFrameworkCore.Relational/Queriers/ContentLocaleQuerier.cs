using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ContentLocaleDto = Krakenar.Contracts.Contents.ContentLocale;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class ContentLocaleQuerier : IContentLocaleQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.ContentLocale> ContentLocales { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public ContentLocaleQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    ContentLocales = context.ContentLocales;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<SearchResults<ContentLocaleDto>> SearchAsync(SearchContentLocalesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.ContentLocales.Table).SelectAll(KrakenarDb.ContentLocales.Table)
      .LeftJoin(Contents.ContentId, KrakenarDb.ContentLocales.ContentId)
      .WhereRealm(ApplicationContext.RealmId, Contents.RealmUid)
      .ApplyIdFilter(Contents.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.ContentLocales.UniqueName, KrakenarDb.ContentLocales.DisplayName);

    if (payload.ContentTypeId.HasValue)
    {
      builder.LeftJoin(ContentTypes.ContentTypeId, KrakenarDb.ContentLocales.ContentTypeId)
        .Where(ContentTypes.Id, Operators.IsEqualTo(payload.ContentTypeId.Value.ToString()));
    }
    if (payload.LanguageId.HasValue)
    {
      builder.LeftJoin(Languages.LanguageId, KrakenarDb.ContentLocales.LanguageId)
        .Where(Languages.Id, Operators.IsEqualTo(payload.LanguageId.Value.ToString()));
    }
    else
    {
      builder.Where(KrakenarDb.ContentLocales.LanguageId, Operators.IsNull());
    }

    IQueryable<Entities.ContentLocale> query = ContentLocales.FromQuery(builder).AsNoTracking()
      .Include(x => x.Content).ThenInclude(x => x!.ContentType)
      .Include(x => x.Language);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.ContentLocale>? ordered = null;
    foreach (ContentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ContentSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ContentSort.DisplayName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ContentSort.UniqueName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case ContentSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.ContentLocale[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ContentLocaleDto> contentLocales = await MapAsync(entities, cancellationToken);

    return new SearchResults<ContentLocaleDto>(contentLocales, total);
  }

  protected virtual async Task<ContentLocaleDto> MapAsync(Entities.ContentLocale content, CancellationToken cancellationToken)
  {
    return (await MapAsync([content], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<ContentLocaleDto>> MapAsync(IEnumerable<Entities.ContentLocale> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return contents.Select(locale => mapper.ToContentLocale(locale, content: null, realm)).ToList().AsReadOnly();
  }
}
