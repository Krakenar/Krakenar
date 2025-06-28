using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class PublishedContentQuerier : IPublishedContentQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual DbSet<Entities.PublishedContent> PublishedContents { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public PublishedContentQuerier(IActorService actorService, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    PublishedContents = context.PublishedContents;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<PublishedContent?> ReadAsync(int contentId, CancellationToken cancellationToken)
  {
    Entities.PublishedContent[] locales = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentId == contentId)
      .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapContentsAsync(locales, cancellationToken)).Single();
  }
  public virtual async Task<PublishedContent?> ReadAsync(Guid contentId, CancellationToken cancellationToken)
  {
    Entities.PublishedContent[] locales = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentUid == contentId)
      .ToArrayAsync(cancellationToken);

    if (locales.Length < 1)
    {
      return null;
    }

    return (await MapContentsAsync(locales, cancellationToken)).Single();
  }

  public virtual async Task<PublishedContent?> ReadAsync(PublishedContentKey key, CancellationToken cancellationToken)
  {
    if (int.TryParse(key.ContentType, out int contentTypeId))
    {
      if (key.Language is not null && int.TryParse(key.Language, out int languageId))
      {
        return await ReadAsync(contentTypeId, languageId, key.UniqueName, cancellationToken);
      }
      else if (key.Language is not null && Guid.TryParse(key.Language, out Guid languageUid))
      {
        return await ReadAsync(contentTypeId, languageUid, key.UniqueName, cancellationToken);
      }
      else
      {
        return await ReadAsync(contentTypeId, key.Language, key.UniqueName, cancellationToken);
      }
    }
    else if (Guid.TryParse(key.ContentType, out Guid contentTypeUid))
    {
      if (key.Language is not null && int.TryParse(key.Language, out int languageId))
      {
        return await ReadAsync(contentTypeUid, languageId, key.UniqueName, cancellationToken);
      }
      else if (key.Language is not null && Guid.TryParse(key.Language, out Guid languageUid))
      {
        return await ReadAsync(contentTypeUid, languageUid, key.UniqueName, cancellationToken);
      }
      else
      {
        return await ReadAsync(contentTypeUid, key.Language, key.UniqueName, cancellationToken);
      }
    }
    else
    {
      if (key.Language is not null && int.TryParse(key.Language, out int languageId))
      {
        return await ReadAsync(key.ContentType, languageId, key.UniqueName, cancellationToken);
      }
      else if (key.Language is not null && Guid.TryParse(key.Language, out Guid languageUid))
      {
        return await ReadAsync(key.ContentType, languageUid, key.UniqueName, cancellationToken);
      }
      else
      {
        return await ReadAsync(key.ContentType, key.Language, key.UniqueName, cancellationToken);
      }
    }
  }

  public virtual async Task<PublishedContent?> ReadAsync(int contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(Guid contentTypeId, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(string contentTypeName, int? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageId == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(int contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(Guid contentTypeId, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(string contentTypeName, Guid? languageId, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageUid == languageId && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(int contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    if (languageCode is not null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeId == contentTypeId && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(Guid contentTypeId, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    if (languageCode is not null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeUid == contentTypeId && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }
  public virtual async Task<PublishedContent?> ReadAsync(string contentTypeName, string? languageCode, string uniqueName, CancellationToken cancellationToken)
  {
    contentTypeName = Helper.Normalize(contentTypeName);
    if (languageCode is not null)
    {
      languageCode = Helper.Normalize(languageCode);
    }
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    int? contentId = await PublishedContents.AsNoTracking()
      .Where(x => x.ContentTypeName == contentTypeName && x.LanguageCode == languageCode && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => (int?)x.ContentId)
      .SingleOrDefaultAsync(cancellationToken);

    return contentId.HasValue ? await ReadAsync(contentId.Value, cancellationToken) : null;
  }

  public virtual async Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.PublishedContents.Table).SelectAll(KrakenarDb.PublishedContents.Table);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.PublishedContents.UniqueName, KrakenarDb.PublishedContents.DisplayName);

    if (payload.Content.Ids.Count > 0)
    {
      object[] contentIds = payload.Content.Ids.Select(id => (object)id).ToArray();
      builder.Where(KrakenarDb.PublishedContents.ContentId, Operators.IsIn(contentIds));
    }
    if (payload.Content.Uids.Count > 0)
    {
      object[] contentUids = payload.Content.Uids.Select(id => (object)id).ToArray();
      builder.Where(KrakenarDb.PublishedContents.ContentUid, Operators.IsIn(contentUids));
    }

    if (payload.ContentType.Ids.Count > 0)
    {
      object[] contentTypeIds = payload.ContentType.Ids.Select(id => (object)id).ToArray();
      builder.Where(KrakenarDb.PublishedContents.ContentTypeId, Operators.IsIn(contentTypeIds));
    }
    if (payload.ContentType.Uids.Count > 0)
    {
      object[] contentTypeUids = payload.ContentType.Uids.Select(id => (object)id).ToArray();
      builder.Where(KrakenarDb.PublishedContents.ContentTypeUid, Operators.IsIn(contentTypeUids));
    }
    if (payload.ContentType.Names.Count > 0)
    {
      string[] contentTypeNames = payload.ContentType.Names.Select(Helper.Normalize).ToArray();
      builder.Where(KrakenarDb.PublishedContents.ContentTypeName, Operators.IsIn(contentTypeNames));
    }

    if (payload.Language.Ids.Count > 0)
    {
      object[] languageIds = payload.Language.Ids.Select(id => (object)id).ToArray();
      builder.Where(KrakenarDb.PublishedContents.LanguageId, Operators.IsIn(languageIds));
    }
    if (payload.Language.Uids.Count > 0)
    {
      object[] languageUids = payload.Language.Uids.Select(id => (object)id).ToArray();
      builder.Where(KrakenarDb.PublishedContents.LanguageUid, Operators.IsIn(languageUids));
    }
    if (payload.Language.Codes.Count > 0)
    {
      string[] languageCodes = payload.Language.Codes.Select(Helper.Normalize).ToArray();
      builder.Where(KrakenarDb.PublishedContents.LanguageCode, Operators.IsIn(languageCodes));
    }
    if (payload.Language.IsDefault.HasValue)
    {
      builder.Where(KrakenarDb.PublishedContents.LanguageIsDefault, Operators.IsEqualTo(payload.Language.IsDefault.Value));
    }

    IQueryable<Entities.PublishedContent> query = PublishedContents.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.PublishedContent>? ordered = null;
    foreach (PublishedContentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case PublishedContentSort.DisplayName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case PublishedContentSort.PublishedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PublishedOn) : query.OrderBy(x => x.PublishedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PublishedOn) : ordered.ThenBy(x => x.PublishedOn));
          break;
        case PublishedContentSort.UniqueName:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload.Skip, payload.Limit);

    Entities.PublishedContent[] publishedContents = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<PublishedContentLocale> items = await MapLocalesAsync(publishedContents, cancellationToken);

    return new SearchResults<PublishedContentLocale>(items, total);
  }

  private async Task<IReadOnlyCollection<PublishedContent>> MapContentsAsync(IEnumerable<Entities.PublishedContent> locales, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Dictionary<int, ContentTypeDto> contentTypes = [];
    Dictionary<int, Language> languages = [];
    Dictionary<int, PublishedContent> publishedContents = [];
    foreach (Entities.PublishedContent locale in locales)
    {
      if (!contentTypes.TryGetValue(locale.ContentTypeId, out ContentTypeDto? contentType))
      {
        contentType = new(locale.ContentTypeName)
        {
          Id = locale.ContentTypeUid
        };
        contentTypes[locale.ContentTypeId] = contentType;
      }

      if (!publishedContents.TryGetValue(locale.ContentId, out PublishedContent? publishedContent))
      {
        publishedContent = new()
        {
          Id = locale.ContentUid,
          ContentType = contentType
        };
        publishedContents[locale.ContentId] = publishedContent;
      }

      Language? language = null;
      if (locale.LanguageId.HasValue && !languages.TryGetValue(locale.LanguageId.Value, out language))
      {
        language = new()
        {
          IsDefault = locale.LanguageIsDefault
        };
        if (locale.LanguageUid.HasValue)
        {
          language.Id = locale.LanguageUid.Value;
        }
        if (locale.LanguageCode is not null)
        {
          language.Locale = new(locale.LanguageCode);
        }
        languages[locale.LanguageId.Value] = language;
      }

      PublishedContentLocale publishedLocale = mapper.ToPublishedContentLocale(locale, publishedContent, language);
      if (language is null)
      {
        publishedContent.Invariant = publishedLocale;
      }
      else
      {
        publishedContent.Locales.Add(publishedLocale);
      }
    }

    return publishedContents.Values;
  }

  private async Task<IReadOnlyCollection<PublishedContentLocale>> MapLocalesAsync(IEnumerable<Entities.PublishedContent> locales, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = locales.SelectMany(locale => locale.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Dictionary<int, ContentTypeDto> contentTypes = [];
    Dictionary<int, Language> languages = [];
    Dictionary<int, PublishedContent> publishedContents = [];
    List<PublishedContentLocale> publishedLocales = [];
    foreach (Entities.PublishedContent locale in locales)
    {
      if (!contentTypes.TryGetValue(locale.ContentTypeId, out ContentTypeDto? contentType))
      {
        contentType = new(locale.ContentTypeName)
        {
          Id = locale.ContentTypeUid
        };
        contentTypes[locale.ContentTypeId] = contentType;
      }

      if (!publishedContents.TryGetValue(locale.ContentId, out PublishedContent? publishedContent))
      {
        publishedContent = new()
        {
          Id = locale.ContentUid,
          ContentType = contentType
        };
        publishedContents[locale.ContentId] = publishedContent;
      }

      Language? language = null;
      if (locale.LanguageId.HasValue && !languages.TryGetValue(locale.LanguageId.Value, out language))
      {
        language = new()
        {
          IsDefault = locale.LanguageIsDefault
        };
        if (locale.LanguageUid.HasValue)
        {
          language.Id = locale.LanguageUid.Value;
        }
        if (locale.LanguageCode is not null)
        {
          language.Locale = new(locale.LanguageCode);
        }
        languages[locale.LanguageId.Value] = language;
      }

      PublishedContentLocale publishedLocale = mapper.ToPublishedContentLocale(locale, publishedContent, language);
      if (language is null)
      {
        publishedContent.Invariant = publishedLocale;
      }
      else
      {
        publishedContent.Locales.Add(publishedLocale);
      }
      publishedLocales.Add(publishedLocale);
    }

    return publishedLocales.AsReadOnly();
  }
}
