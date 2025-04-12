using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using RoleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Role;
using UserEntity = Krakenar.EntityFrameworkCore.Relational.Entities.User;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class UserEvents : IEventHandler<UserAddressChanged>,
  IEventHandler<UserAuthenticated>,
  IEventHandler<UserCreated>,
  IEventHandler<UserDeleted>,
  IEventHandler<UserDisabled>,
  IEventHandler<UserEmailChanged>,
  IEventHandler<UserEnabled>,
  IEventHandler<UserIdentifierChanged>,
  IEventHandler<UserIdentifierRemoved>,
  IEventHandler<UserPasswordChanged>,
  IEventHandler<UserPasswordRemoved>,
  IEventHandler<UserPasswordReset>,
  IEventHandler<UserPasswordUpdated>,
  IEventHandler<UserPhoneChanged>,
  IEventHandler<UserRoleAdded>,
  IEventHandler<UserRoleRemoved>,
  IEventHandler<UserSignedIn>,
  IEventHandler<UserUniqueNameChanged>,
  IEventHandler<UserUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<UserEvents> Logger { get; }

  public UserEvents(KrakenarContext context, ILogger<UserEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(UserAddressChanged @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetAddress(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserAuthenticated @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.Authenticate(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
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

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, user);
    }
  }

  public virtual async Task HandleAsync(UserDeleted @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Users.Remove(user);

      await SaveActorAsync(user, isDeleted: true, cancellationToken);
      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(UserDisabled @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.Disable(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserEmailChanged @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetEmail(@event);

    await SaveActorAsync(user, cancellationToken);
    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserEnabled @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.Enable(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserIdentifierChanged @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users
      .Include(x => x.Identifiers)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetCustomIdentifier(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserIdentifierRemoved @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users
      .Include(x => x.Identifiers)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.RemoveCustomIdentifier(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserPasswordChanged @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetPassword(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserPasswordRemoved @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.RemovePassword(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserPasswordReset @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetPassword(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserPasswordUpdated @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetPassword(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserPhoneChanged @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetPhone(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserRoleAdded @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    RoleEntity role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.RoleId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The role entity 'StreamId={@event.RoleId}' could not be found.");

    user.AddRole(role, @event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserRoleRemoved @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.RemoveRole(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserSignedIn @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SignIn(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.SetUniqueName(@event);

    await SaveActorAsync(user, cancellationToken);
    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(UserUpdated @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await Context.Users.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (user is null || user.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, user);
      return;
    }

    user.Update(@event);

    await SaveActorAsync(user, cancellationToken);
    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  protected virtual async Task SaveActorAsync(UserEntity user, CancellationToken cancellationToken)
  {
    await SaveActorAsync(user, isDeleted: null, cancellationToken);
  }
  protected virtual async Task SaveActorAsync(UserEntity user, bool? isDeleted, CancellationToken cancellationToken)
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

    if (isDeleted.HasValue)
    {
      actor.IsDeleted = isDeleted.Value;
    }
  }
}
