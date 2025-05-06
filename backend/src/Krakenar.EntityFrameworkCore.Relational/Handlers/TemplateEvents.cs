using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Templates;
using Krakenar.Core.Templates.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using TemplateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Template;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class TemplateEvents : IEventHandler<TemplateCreated>, IEventHandler<TemplateDeleted>, IEventHandler<TemplateUniqueNameChanged>, IEventHandler<TemplateUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<TemplateEvents> Logger { get; }

  public TemplateEvents(KrakenarContext context, ILogger<TemplateEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(TemplateCreated @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await Context.Templates.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (template is null)
    {
      RealmId? realmId = new TemplateId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      template = new TemplateEntity(realm, @event);

      Context.Templates.Add(template);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, template);
    }
  }

  public virtual async Task HandleAsync(TemplateDeleted @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await Context.Templates.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (template is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Templates.Remove(template);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(TemplateUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await Context.Templates.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (template is null || template.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, template);
      return;
    }

    template.SetUniqueName(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(TemplateUpdated @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await Context.Templates.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (template is null || template.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, template);
      return;
    }

    template.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
