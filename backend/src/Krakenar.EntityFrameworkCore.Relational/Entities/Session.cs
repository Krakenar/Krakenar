using Krakenar.Core.Sessions;
using Krakenar.Core.Sessions.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Session : Aggregate, ISegregatedEntity
{
  public int SessionId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public User? User { get; private set; }
  public int UserId { get; private set; }
  public Guid UserUid { get; private set; }

  public string? SecretHash { get; private set; }
  public bool IsPersistent
  {
    get => SecretHash is not null;
    private set { }
  }

  public string? SignedOutBy { get; private set; }
  public DateTime? SignedOutOn { get; private set; }
  public bool IsActive
  {
    get => !SignedOutOn.HasValue;
    private set { }
  }

  public string? CustomAttributes { get; private set; }

  public Session(User user, SessionCreated @event) : base(@event)
  {
    Realm = user.Realm;
    RealmId = user.RealmId;
    RealmUid = user.RealmUid;

    Id = new SessionId(@event.StreamId).EntityId;

    User = user;
    UserId = user.UserId;
    UserUid = user.Id;

    SecretHash = @event.Secret?.Encode();
  }

  private Session() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(skipUser: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool skipUser)
  {
    List<ActorId> actorIds = [];
    actorIds.AddRange(base.GetActorIds());

    if (SignedOutBy is not null)
    {
      actorIds.Add(new ActorId(SignedOutBy));
    }

    if (!skipUser && User is not null)
    {
      actorIds.AddRange(User.GetActorIds(skipRoles: false, skipSessions: true));
    }

    return actorIds.AsReadOnly();
  }

  public void Renew(SessionRenewed @event)
  {
    Update(@event);

    SecretHash = @event.Secret.Encode();
  }

  public void SignOut(SessionSignedOut @event)
  {
    Update(@event);

    SignedOutBy = @event.ActorId?.Value;
    SignedOutOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Update(SessionUpdated @event)
  {
    base.Update(@event);

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
}
