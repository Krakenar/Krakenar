using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Language = Krakenar.Core.Localization.Language;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using Locale = Krakenar.Core.Localization.Locale;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class LanguageQuerier : ILanguageQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Language> Languages { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public LanguageQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Languages = context.Languages;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<Locale> FindPlatformDefaultLocaleAsync(CancellationToken cancellationToken)
  {
    string code = await Languages.AsNoTracking()
      .Where(x => x.RealmId == null)
      .Select(x => x.Code)
      .SingleOrDefaultAsync(cancellationToken)
      ?? throw new InvalidOperationException("The platform default locale could not be found.");

    return new Locale(code);
  }

  public virtual async Task<LanguageId> FindDefaultIdAsync(CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;

    string streamId = await Languages.AsNoTracking()
      .WhereRealm(realmId)
      .Where(x => x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken)
      ?? throw new InvalidOperationException($"The default language for realm 'Id={realmId?.Value ?? "<null>"}' could not be found.");

    return new LanguageId(streamId);
  }

  public virtual async Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken)
  {
    string codeNormalized = Helper.Normalize(locale);

    string? streamId = await Languages.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.CodeNormalized == codeNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new LanguageId(streamId);
  }

  public virtual async Task<LanguageDto> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken) ?? throw new InvalidOperationException($"The language entity 'StreamId={language.Id}' could not be found.");
  }
  public virtual async Task<LanguageDto?> ReadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<LanguageDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Language? language = await Languages.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return language is null ? null : await MapAsync(language, cancellationToken);
  }
  public virtual async Task<LanguageDto?> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    string codeNormalized = Helper.Normalize(locale);

    Entities.Language? language = await Languages.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.CodeNormalized == codeNormalized, cancellationToken);

    return language is null ? null : await MapAsync(language, cancellationToken);
  }

  public virtual async Task<LanguageDto> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;

    Entities.Language language = await Languages.AsNoTracking()
      .WhereRealm(realmId)
      .SingleOrDefaultAsync(x => x.IsDefault, cancellationToken)
      ?? throw new InvalidOperationException($"The default language entity for realm 'Id={realmId?.Value ?? "<null>"}' could not be found.");

    return await MapAsync(language, cancellationToken);
  }

  public virtual async Task<SearchResults<LanguageDto>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Languages.Table).SelectAll(KrakenarDb.Languages.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Languages.RealmUid)
      .ApplyIdFilter(KrakenarDb.Languages.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.Languages.Code, KrakenarDb.Languages.DisplayName, KrakenarDb.Languages.EnglishName, KrakenarDb.Languages.NativeName);

    IQueryable<Entities.Language> query = Languages.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Language>? ordered = null;
    foreach (LanguageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LanguageSort.Code:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Code) : ordered.ThenBy(x => x.Code));
          break;
        case LanguageSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LanguageSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case LanguageSort.EnglishName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EnglishName) : query.OrderBy(x => x.EnglishName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EnglishName) : ordered.ThenBy(x => x.EnglishName));
          break;
        case LanguageSort.NativeName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.NativeName) : query.OrderBy(x => x.NativeName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.NativeName) : ordered.ThenBy(x => x.NativeName));
          break;
        case LanguageSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Language[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<LanguageDto> languages = await MapAsync(entities, cancellationToken);

    return new SearchResults<LanguageDto>(languages, total);
  }

  protected virtual async Task<LanguageDto> MapAsync(Entities.Language language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<LanguageDto>> MapAsync(IEnumerable<Entities.Language> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    RealmDto? realm = ApplicationContext.Realm;
    return languages.Select(language => mapper.ToLanguage(language, realm)).ToList().AsReadOnly();
  }
}
