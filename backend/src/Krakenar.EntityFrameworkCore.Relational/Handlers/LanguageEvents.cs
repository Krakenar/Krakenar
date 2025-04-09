using Krakenar.Core;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Microsoft.EntityFrameworkCore;
using LanguageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Language;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class LanguageEvents : IEventHandler<LanguageCreated>, IEventHandler<LanguageDeleted>, IEventHandler<LanguageLocaleChanged>, IEventHandler<LanguageSetDefault>
{
  protected virtual KrakenarContext Context { get; }

  public LanguageEvents(KrakenarContext context)
  {
    Context = context;
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
      // TODO(fpion): report
    }
    else
    {
      // TODO(fpion): report
    }
  }

  public virtual async Task HandleAsync(LanguageDeleted @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language is null)
    {
      // TODO(fpion): report
    }
    else
    {
      Context.Languages.Remove(language);

      await Context.SaveChangesAsync(cancellationToken);
      // TODO(fpion): report
    }
  }

  public virtual async Task HandleAsync(LanguageLocaleChanged @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language is null)
    {
      return; // TODO(fpion): report
    }

    long expectedVersion = @event.Version - 1;
    if (language.Version < expectedVersion)
    {
      return; // TODO(fpion): report
    }
    else if (language.Version > expectedVersion)
    {
      return; // TODO(fpion): report
    }

    language.SetLocale(@event);

    await Context.SaveChangesAsync(cancellationToken);
    // TODO(fpion): report
  }

  public virtual async Task HandleAsync(LanguageSetDefault @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language is null)
    {
      return; // TODO(fpion): report
    }

    long expectedVersion = @event.Version - 1;
    if (language.Version < expectedVersion)
    {
      return; // TODO(fpion): report
    }
    else if (language.Version > expectedVersion)
    {
      return; // TODO(fpion): report
    }

    language.SetDefault(@event);

    await Context.SaveChangesAsync(cancellationToken);
    // TODO(fpion): report
  }
}
