using Krakenar.Core;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LanguageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Language;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class LanguageEvents : IEventHandler<LanguageCreated>, IEventHandler<LanguageDeleted>, IEventHandler<LanguageLocaleChanged>, IEventHandler<LanguageSetDefault>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<LanguageEvents> Logger { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public LanguageEvents(KrakenarContext context, ILogger<LanguageEvents> logger, ISqlHelper sqlHelper)
  {
    Context = context;
    Logger = logger;
    SqlHelper = sqlHelper;
  }

  public virtual async Task HandleAsync(LanguageCreated @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await Context.Languages.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language is null)
    {
      RealmId? realmId = new LanguageId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      language = new LanguageEntity(realm, @event);

      Context.Languages.Add(language);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, language);
    }
  }

  public virtual async Task HandleAsync(LanguageDeleted @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Languages.Remove(language);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(LanguageLocaleChanged @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language is null || language.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event);
      return;
    }

    language.SetLocale(@event);

    await Context.SaveChangesAsync(cancellationToken);

    await Context.PublishedContents
      .Where(x => x.LanguageId == language.LanguageId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LanguageCode, language.CodeNormalized), cancellationToken);

    await Context.FieldIndex
      .Where(x => x.LanguageId == language.LanguageId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LanguageCode, language.CodeNormalized), cancellationToken);

    await Context.UniqueIndex
      .Where(x => x.LanguageId == language.LanguageId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LanguageCode, language.CodeNormalized), cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(LanguageSetDefault @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language is null || language.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event);
      return;
    }

    language.SetDefault(@event);

    await Context.SaveChangesAsync(cancellationToken);

    await Context.PublishedContents
      .Where(x => x.LanguageId == language.LanguageId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LanguageIsDefault, language.IsDefault), cancellationToken);

    await Context.FieldIndex
      .Where(x => x.LanguageId == language.LanguageId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LanguageIsDefault, language.IsDefault), cancellationToken);

    await Context.UniqueIndex
      .Where(x => x.LanguageId == language.LanguageId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LanguageIsDefault, language.IsDefault), cancellationToken);

    Logger.LogSuccess(@event);
  }
}
