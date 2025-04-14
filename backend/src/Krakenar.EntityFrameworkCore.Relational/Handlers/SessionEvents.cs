using Krakenar.Core;
using Krakenar.Core.Sessions.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SessionEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Session;
using UserEntity = Krakenar.EntityFrameworkCore.Relational.Entities.User;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class SessionEvents : IEventHandler<SessionCreated>,
  IEventHandler<SessionDeleted>,
  IEventHandler<SessionRenewed>,
  IEventHandler<SessionSignedOut>,
  IEventHandler<SessionUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<SessionEvents> Logger { get; }

  public SessionEvents(KrakenarContext context, ILogger<SessionEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(SessionCreated @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await Context.Sessions.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (session is null)
    {
      UserEntity user = await Context.Users
        .SingleOrDefaultAsync(x => x.StreamId == @event.UserId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The user entity 'StreamId={@event.UserId}' could not be found.");

      session = new SessionEntity(user, @event);

      Context.Sessions.Add(session);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, session);
    }
  }

  public virtual async Task HandleAsync(SessionDeleted @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await Context.Sessions.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (session is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Sessions.Remove(session);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(SessionRenewed @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await Context.Sessions.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (session is null || session.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, session);
      return;
    }

    session.Renew(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(SessionSignedOut @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await Context.Sessions.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (session is null || session.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, session);
      return;
    }

    session.SignOut(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(SessionUpdated @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await Context.Sessions.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (session is null || session.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, session);
      return;
    }

    session.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
