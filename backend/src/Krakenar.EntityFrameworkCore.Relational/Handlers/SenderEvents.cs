using Krakenar.Core.Realms;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Events;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using SenderEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Sender;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class SenderEvents : IEventHandler<EmailSenderCreated>,
  IEventHandler<PhoneSenderCreated>,
  IEventHandler<SenderDeleted>,
  IEventHandler<SenderSetDefault>,
  IEventHandler<SenderUpdated>,
  IEventHandler<SendGridSettingsChanged>,
  IEventHandler<TwilioSettingsChanged>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<SenderEvents> Logger { get; }

  public SenderEvents(KrakenarContext context, ILogger<SenderEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(EmailSenderCreated @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await Context.Senders.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender is null)
    {
      RealmId? realmId = new SenderId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      sender = new SenderEntity(realm, @event);

      Context.Senders.Add(sender);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, sender);
    }
  }

  public virtual async Task HandleAsync(PhoneSenderCreated @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await Context.Senders.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender is null)
    {
      RealmId? realmId = new SenderId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      sender = new SenderEntity(realm, @event);

      Context.Senders.Add(sender);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, sender);
    }
  }

  public virtual async Task HandleAsync(SenderDeleted @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await Context.Senders.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Senders.Remove(sender);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(SenderSetDefault @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await Context.Senders.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender is null || sender.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, sender);
      return;
    }

    sender.SetDefault(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(SenderUpdated @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await Context.Senders.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender is null || sender.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, sender);
      return;
    }

    sender.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(SendGridSettingsChanged @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await Context.Senders.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender is null || sender.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, sender);
      return;
    }

    sender.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(TwilioSettingsChanged @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await Context.Senders.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender is null || sender.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, sender);
      return;
    }

    sender.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
