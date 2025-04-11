using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Events;
using Microsoft.EntityFrameworkCore;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using UserEntity = Krakenar.EntityFrameworkCore.Relational.Entities.User;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class UserEvents : IEventHandler<UserCreated> // TODO(fpion): handle other events
{
  protected virtual KrakenarContext Context { get; }

  public UserEvents(KrakenarContext context)
  {
    Context = context;
  }

  public virtual async Task HandleAsync(UserCreated @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null)
    {
      RealmId? realmId = new UserId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      user = new UserEntity(realm, @event);

      Context.Users.Add(user);

      await SaveActorAsync(user, cancellationToken);
      await Context.SaveChangesAsync(cancellationToken);
      // TODO(fpion): report
    }
    else
    {
      // TODO(fpion): report
    }
  }

  protected virtual async Task SaveActorAsync(UserEntity user, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await Context.Actors.SingleOrDefaultAsync(x => x.Key == user.StreamId, cancellationToken);
    if (actor is null)
    {
      actor = new ActorEntity(user);

      Context.Actors.Add(actor);
    }
    else
    {
      actor.Update(user);
    }
  }
}
