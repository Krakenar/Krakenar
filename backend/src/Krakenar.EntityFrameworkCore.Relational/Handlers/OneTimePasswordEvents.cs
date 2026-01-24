using Krakenar.Core.Passwords;
using Krakenar.Core.Passwords.Events;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneTimePasswordEntity = Krakenar.EntityFrameworkCore.Relational.Entities.OneTimePassword;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using UserEntity = Krakenar.EntityFrameworkCore.Relational.Entities.User;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

internal class OneTimePasswordEvents : IEventHandler<OneTimePasswordCreated>,
  IEventHandler<OneTimePasswordDeleted>,
  IEventHandler<OneTimePasswordUpdated>,
  IEventHandler<OneTimePasswordValidationFailed>,
  IEventHandler<OneTimePasswordValidationSucceeded>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<OneTimePasswordEvents> Logger { get; }

  public OneTimePasswordEvents(KrakenarContext context, ILogger<OneTimePasswordEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(OneTimePasswordCreated @event, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await Context.OneTimePasswords.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (oneTimePassword is null)
    {
      if (@event.UserId.HasValue)
      {
        UserEntity user = await Context.Users
          .Include(x => x.Realm)
          .SingleOrDefaultAsync(x => x.StreamId == @event.UserId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The user entity 'StreamId={@event.UserId}' could not be found.");

        oneTimePassword = new OneTimePasswordEntity(user, @event);
      }
      else
      {
        RealmId? realmId = new OneTimePasswordId(@event.StreamId).RealmId;
        RealmEntity? realm = realmId.HasValue
          ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
            ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
          : null;

        oneTimePassword = new OneTimePasswordEntity(realm, @event);
      }

      Context.OneTimePasswords.Add(oneTimePassword);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, oneTimePassword);
    }
  }

  public virtual async Task HandleAsync(OneTimePasswordDeleted @event, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await Context.OneTimePasswords.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (oneTimePassword is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.OneTimePasswords.Remove(oneTimePassword);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(OneTimePasswordUpdated @event, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await Context.OneTimePasswords.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (oneTimePassword is null || oneTimePassword.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event);
      return;
    }

    oneTimePassword.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(OneTimePasswordValidationFailed @event, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await Context.OneTimePasswords.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (oneTimePassword is null || oneTimePassword.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event);
      return;
    }

    oneTimePassword.Fail(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(OneTimePasswordValidationSucceeded @event, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await Context.OneTimePasswords.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (oneTimePassword is null || oneTimePassword.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event);
      return;
    }

    oneTimePassword.Succeed(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
