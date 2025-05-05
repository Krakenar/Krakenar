using Krakenar.Core.ApiKeys;
using Krakenar.Core.ApiKeys.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class ApiKey : Aggregate, ISegregatedEntity
{
  public int ApiKeyId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public string SecretHash { get; private set; } = string.Empty;

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }
  public DateTime? ExpiresOn { get; private set; }

  public DateTime? AuthenticatedOn { get; private set; }

  public string? CustomAttributes { get; private set; }

  public List<Role> Roles { get; private set; } = [];

  public ApiKey(Realm? realm, ApiKeyCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Id = new ApiKeyId(@event.StreamId).EntityId;

    SecretHash = @event.Secret.Encode();

    Name = @event.Name.Value;
  }

  private ApiKey() : base()
  {
  }

  public void AddRole(Role role, ApiKeyRoleAdded @event)
  {
    Update(@event);

    Roles.Add(role);
  }

  public void Authenticate(ApiKeyAuthenticated @event)
  {
    Update(@event);

    AuthenticatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipRoles: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipRoles)
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    if (!skipRoles)
    {
      foreach (Role role in Roles)
      {
        actorIds.AddRange(role.GetActorIds());
      }
    }
    return actorIds.ToList().AsReadOnly();
  }

  public void RemoveRole(ApiKeyRoleRemoved @event)
  {
    Update(@event);

    Role? role = Roles.SingleOrDefault(x => x.StreamId == @event.RoleId.StreamId.Value);
    if (role is not null)
    {
      Roles.Remove(role);
    }
  }

  public void Update(ApiKeyUpdated @event)
  {
    base.Update(@event);

    if (@event.Name is not null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }
    if (@event.ExpiresOn.HasValue)
    {
      ExpiresOn = @event.ExpiresOn.Value;
    }

    Dictionary<string, string> customAttributes = GetCustomAttributes();
    foreach (KeyValuePair<Core.Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value is null)
      {
        customAttributes.Remove(customAttribute.Key.Value);
      }
      else
      {
        customAttributes[customAttribute.Key.Value] = customAttribute.Value;
      }
    }
    SetCustomAttributes(customAttributes);
  }

  public Dictionary<string, string> GetCustomAttributes()
  {
    return (CustomAttributes is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomAttributes)) ?? [];
  }
  private void SetCustomAttributes(Dictionary<string, string> customAttributes)
  {
    CustomAttributes = customAttributes.Count < 1 ? null : JsonSerializer.Serialize(customAttributes);
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
