using Krakenar.Contracts.Actors;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class LanguageQuerier : ILanguageQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Language> Languages { get; }

  public LanguageQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Languages = context.Languages;
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
