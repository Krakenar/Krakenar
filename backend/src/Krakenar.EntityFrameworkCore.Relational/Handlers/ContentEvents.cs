using Krakenar.Core;
using Krakenar.Core.Contents.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ContentEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Content;
using ContentLocaleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.ContentLocale;
using ContentTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.ContentType;
using LanguageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Language;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class ContentEvents : IEventHandler<ContentCreated>,
  IEventHandler<ContentDeleted>,
  IEventHandler<ContentLocaleChanged>,
  IEventHandler<ContentLocaleRemoved>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<ContentTypeEvents> Logger { get; }

  public ContentEvents(KrakenarContext context, ILogger<ContentTypeEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(ContentCreated @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null)
    {
      ContentTypeEntity contentType = await Context.ContentTypes
        .SingleOrDefaultAsync(x => x.StreamId == @event.ContentTypeId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'StreamId={@event.ContentTypeId}' could not be found.");

      content = new ContentEntity(contentType, @event);

      Context.Contents.Add(content);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, content);
    }
  }

  public virtual async Task HandleAsync(ContentDeleted @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Contents.Remove(content);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(ContentLocaleChanged @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null || content.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, content);
      return;
    }

    LanguageEntity? language = @event.LanguageId.HasValue
      ? (await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' could not be found."))
      : null;

    content.SetLocale(language, @event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ContentLocaleRemoved @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null || content.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, content);
      return;
    }

    ContentLocaleEntity? locale = content.RemoveLocale(@event);
    if (locale is not null)
    {
      this.Context.ContentLocales.Remove(locale);
    }

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
