using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions;

public class Session : AggregateRoot
{
  private SessionUpdated _updated = new();
  private bool HasUpdates => _updated.CustomAttributes.Count > 0;

  public new SessionId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public UserId UserId { get; private set; }

  private Password? _secret = null;
  public bool IsPersistent => _secret is not null;

  public bool IsActive { get; private set; }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  public Session() : base()
  {
  }

  public Session(User user, Password? secret = null, ActorId? actorId = null, SessionId? sessionId = null)
    : base((sessionId ?? SessionId.NewId()).StreamId)
  {
    // TODO(fpion): ensure user is in same realm

    Raise(new SessionCreated(user.Id, secret), actorId);
  }
  protected virtual void Handle(SessionCreated @event)
  {
    UserId = @event.UserId;

    _secret = @event.Secret;

    IsActive = true;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SessionDeleted(), actorId);
    }
  }

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updated.CustomAttributes[key] = null;
    }
  }

  public void Renew(string currentSecret, Password newSecret, ActorId? actorId = default)
  {
    if (!IsActive)
    {
      throw new SessionIsNotActiveException(this);
    }
    else if (_secret is null)
    {
      throw new SessionIsNotPersistentException(this);
    }
    else if (!_secret.IsMatch(currentSecret))
    {
      throw new IncorrectSessionSecretException(this, currentSecret);
    }

    actorId ??= new(UserId.Value);
    Raise(new SessionRenewed(newSecret), actorId.Value);
  }
  protected virtual void Handle(SessionRenewed @event)
  {
    _secret = @event.Secret;
  }

  public void SetCustomAttribute(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveCustomAttribute(key);
    }
    else
    {
      value = value.Trim();
      if (!_customAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
      {
        _customAttributes[key] = value;
        _updated.CustomAttributes[key] = value;
      }
    }
  }

  public void SignOut(ActorId? actorId = default)
  {
    if (IsActive)
    {
      actorId ??= new(UserId.Value);
      Raise(new SessionSignedOut(), actorId.Value);
    }
  }
  protected virtual void Handle(SessionSignedOut _)
  {
    IsActive = false;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(SessionUpdated @event)
  {
    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value is null)
      {
        _customAttributes.Remove(customAttribute.Key);
      }
      else
      {
        _customAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }
  }
}
