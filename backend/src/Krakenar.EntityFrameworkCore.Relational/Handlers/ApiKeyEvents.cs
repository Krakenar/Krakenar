using Krakenar.Core;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.ApiKeys.Events;
using Krakenar.Core.Caching;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;
using ApiKeyEntity = Krakenar.EntityFrameworkCore.Relational.Entities.ApiKey;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using RoleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Role;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class ApiKeyEvents : IEventHandler<ApiKeyAuthenticated>,
  IEventHandler<ApiKeyCreated>,
  IEventHandler<ApiKeyDeleted>,
  IEventHandler<ApiKeyRoleAdded>,
  IEventHandler<ApiKeyRoleRemoved>,
  IEventHandler<ApiKeyUpdated>
{
  protected virtual ICacheService CacheService { get; }
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<ApiKeyEvents> Logger { get; }

  public ApiKeyEvents(ICacheService cacheService, KrakenarContext context, ILogger<ApiKeyEvents> logger)
  {
    CacheService = cacheService;
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(ApiKeyAuthenticated @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await Context.ApiKeys.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (apiKey is null || apiKey.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, apiKey);
      return;
    }

    apiKey.Authenticate(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ApiKeyCreated @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await Context.ApiKeys.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (apiKey is null)
    {
      RealmId? realmId = new ApiKeyId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      apiKey = new ApiKeyEntity(realm, @event);

      Context.ApiKeys.Add(apiKey);

      await SaveActorAsync(apiKey, cancellationToken);
      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, apiKey);
    }
  }

  public virtual async Task HandleAsync(ApiKeyDeleted @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await Context.ApiKeys.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (apiKey is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.ApiKeys.Remove(apiKey);

      await SaveActorAsync(apiKey, isDeleted: true, cancellationToken);
      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(ApiKeyRoleAdded @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await Context.ApiKeys
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (apiKey is null || apiKey.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, apiKey);
      return;
    }

    RoleEntity role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.RoleId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The role entity 'StreamId={@event.RoleId}' could not be found.");

    apiKey.AddRole(role, @event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ApiKeyRoleRemoved @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await Context.ApiKeys
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (apiKey is null || apiKey.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, apiKey);
      return;
    }

    apiKey.RemoveRole(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ApiKeyUpdated @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await Context.ApiKeys.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (apiKey is null || apiKey.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, apiKey);
      return;
    }

    apiKey.Update(@event);

    await SaveActorAsync(apiKey, cancellationToken);
    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  protected virtual async Task SaveActorAsync(ApiKeyEntity apiKey, CancellationToken cancellationToken)
  {
    await SaveActorAsync(apiKey, isDeleted: null, cancellationToken);
  }
  protected virtual async Task SaveActorAsync(ApiKeyEntity apiKey, bool? isDeleted, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await Context.Actors.SingleOrDefaultAsync(x => x.Key == apiKey.StreamId, cancellationToken);
    if (actor is null)
    {
      actor = new ActorEntity(apiKey);

      Context.Actors.Add(actor);
    }
    else
    {
      actor.Update(apiKey);
    }

    if (isDeleted.HasValue)
    {
      actor.IsDeleted = isDeleted.Value;
    }

    CacheService.RemoveActor(new ActorId(actor.Key));
  }
}
