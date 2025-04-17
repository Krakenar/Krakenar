using Krakenar.Core;
using Krakenar.Core.Realms.Events;
using Krakenar.Core.Roles.Events;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CustomAttributeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.CustomAttribute;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class CustomAttributeEvents : IEventHandler<RealmDeleted>,
  IEventHandler<RealmUpdated>,
  IEventHandler<RoleDeleted>,
  IEventHandler<RoleUpdated>,
  IEventHandler<SessionDeleted>,
  IEventHandler<SessionUpdated>,
  IEventHandler<UserDeleted>,
  IEventHandler<UserUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<CustomAttributeEvents> Logger { get; }

  public CustomAttributeEvents(KrakenarContext context, ILogger<CustomAttributeEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(RealmDeleted @event, CancellationToken cancellationToken)
  {
    await DeleteAsync(@event, cancellationToken);
  }

  public virtual async Task HandleAsync(RealmUpdated @event, CancellationToken cancellationToken)
  {
    await SynchronizeAsync(@event, @event.CustomAttributes, cancellationToken);
  }

  public virtual async Task HandleAsync(RoleDeleted @event, CancellationToken cancellationToken)
  {
    await DeleteAsync(@event, cancellationToken);
  }

  public virtual async Task HandleAsync(RoleUpdated @event, CancellationToken cancellationToken)
  {
    await SynchronizeAsync(@event, @event.CustomAttributes, cancellationToken);
  }

  public virtual async Task HandleAsync(SessionDeleted @event, CancellationToken cancellationToken)
  {
    await DeleteAsync(@event, cancellationToken);
  }

  public virtual async Task HandleAsync(SessionUpdated @event, CancellationToken cancellationToken)
  {
    await SynchronizeAsync(@event, @event.CustomAttributes, cancellationToken);
  }

  public virtual async Task HandleAsync(UserDeleted @event, CancellationToken cancellationToken)
  {
    await DeleteAsync(@event, cancellationToken);
  }

  public virtual async Task HandleAsync(UserUpdated @event, CancellationToken cancellationToken)
  {
    await SynchronizeAsync(@event, @event.CustomAttributes, cancellationToken);
  }

  protected virtual async Task DeleteAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    await DeleteAsync(@event.StreamId.Value, cancellationToken);
    Logger.LogSuccess(@event);
  }
  protected virtual async Task DeleteAsync(string entity, CancellationToken cancellationToken)
  {
    await Context.CustomAttributes.Where(x => x.Entity == entity).ExecuteDeleteAsync(cancellationToken);
  }

  protected virtual async Task SynchronizeAsync(DomainEvent @event, IEnumerable<KeyValuePair<Identifier, string?>> changes, CancellationToken cancellationToken)
  {
    await SynchronizeAsync(@event.StreamId.Value, changes, cancellationToken);
    Logger.LogSuccess(@event);
  }
  protected virtual async Task SynchronizeAsync(string entity, IEnumerable<KeyValuePair<Identifier, string?>> changes, CancellationToken cancellationToken)
  {
    Dictionary<string, CustomAttributeEntity> entities = await Context.CustomAttributes
      .Where(x => x.Entity == entity)
      .ToDictionaryAsync(x => x.Key, x => x, cancellationToken);

    foreach (KeyValuePair<Identifier, string?> change in changes)
    {
      _ = entities.TryGetValue(change.Key.Value, out CustomAttributeEntity? customAttribute);

      if (change.Value is null)
      {
        if (customAttribute is not null)
        {
          Context.CustomAttributes.Remove(customAttribute);
        }
      }
      else
      {
        if (customAttribute is null)
        {
          customAttribute = new CustomAttributeEntity(entity, change.Key.Value);
          Context.CustomAttributes.Add(customAttribute);
        }
        customAttribute.Value = change.Value;
      }
    }

    await Context.SaveChangesAsync(cancellationToken);
  }
}
