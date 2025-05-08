using Krakenar.Core;
using Krakenar.Core.Messages;
using Krakenar.Core.Messages.Events;
using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MessageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Message;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class MessageEvents : IEventHandler<MessageCreated>, IEventHandler<MessageDeleted>, IEventHandler<MessageFailed>, IEventHandler<MessageSucceeded>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<MessageEvents> Logger { get; }

  public MessageEvents(KrakenarContext context, ILogger<MessageEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(MessageCreated @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await Context.Messages.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (message is null)
    {
      RealmId? realmId = new MessageId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      Sender sender = await Context.Senders
        .SingleOrDefaultAsync(x => x.StreamId == @event.Sender.Id.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The sender entity 'StreamId={@event.Sender.Id}' could not be found.");

      Template template = await Context.Templates
        .SingleOrDefaultAsync(x => x.StreamId == @event.Template.Id.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The template entity 'StreamId={@event.Template.Id}' could not be found.");

      HashSet<string> userIds = @event.Recipients.Where(x => x.UserId.HasValue).Select(x => x.UserId!.Value.Value).ToHashSet();
      Dictionary<string, User> users = await Context.Users
        .Where(x => userIds.Contains(x.StreamId))
        .ToDictionaryAsync(x => x.StreamId, x => x, cancellationToken);

      message = new MessageEntity(realm, sender, template, users, @event);

      Context.Messages.Add(message);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, message);
    }
  }

  public virtual async Task HandleAsync(MessageDeleted @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await Context.Messages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (message is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Messages.Remove(message);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(MessageFailed @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await Context.Messages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (message is null || message.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, message);
      return;
    }

    message.Fail(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(MessageSucceeded @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await Context.Messages.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (message is null || message.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, message);
      return;
    }

    message.Succeed(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
