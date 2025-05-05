using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core.ApiKeys.Events;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys;

public class ApiKey : AggregateRoot, ICustomizable
{
  private Password? _secret = null;
  private ApiKeyUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Description is not null || _updated.ExpiresOn is not null
    || _updated.CustomAttributes.Count > 0;

  public new ApiKeyId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private DisplayName? _name = null;
  public DisplayName Name
  {
    get => _name ?? throw new InvalidOperationException("The API key has not been initialized.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updated.Name = value;
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }
  private DateTime? _expiresOn = null;
  public DateTime? ExpiresOn
  {
    get => _expiresOn;
    set
    {
      if (_expiresOn != value)
      {
        if (!value.HasValue || (_expiresOn.HasValue && _expiresOn.Value.AsUniversalTime() < value.Value.AsUniversalTime()))
        {
          ValidationFailure failure = new(nameof(ExpiresOn), "The API key expiration cannot be extended, nor removed.", value)
          {
            ErrorCode = "ExpirationValidator"
          };
          throw new ValidationException([failure]);
        }
        else if (value.Value.AsUniversalTime() < DateTime.UtcNow)
        {
          throw new ArgumentOutOfRangeException(nameof(ExpiresOn), "The value must be a date and time set in the future.");
        }

        _expiresOn = value;
        _updated.ExpiresOn = value;
      }
    }
  }

  public DateTime? AuthenticatedOn { get; private set; }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  private readonly HashSet<RoleId> _roles = [];
  public IReadOnlyCollection<RoleId> Roles => _roles.ToList().AsReadOnly();

  public ApiKey() : base()
  {
  }

  public ApiKey(Password secret, DisplayName name, ActorId? actorId = null, ApiKeyId? apiKeyId = null)
    : base((apiKeyId ?? ApiKeyId.NewId()).StreamId)
  {
    Raise(new ApiKeyCreated(secret, name), actorId);
  }
  protected virtual void Handle(ApiKeyCreated @event)
  {
    _secret = @event.Secret;

    _name = @event.Name;
  }

  public void AddRole(Role role, ActorId? actorId = null)
  {
    if (RealmId != role.RealmId)
    {
      throw new RealmMismatchException(RealmId, role.RealmId, nameof(role));
    }

    if (!HasRole(role))
    {
      Raise(new ApiKeyRoleAdded(role.Id), actorId);
    }
  }
  protected virtual void Handle(ApiKeyRoleAdded @event)
  {
    _roles.Add(@event.RoleId);
  }

  public void Authenticate(string secret, ActorId? actorId = null)
  {
    if (IsExpired())
    {
      throw new ApiKeyIsExpiredException(this);
    }
    else if (_secret is null || !_secret.IsMatch(secret))
    {
      throw new IncorrectApiKeySecretException(this, secret);
    }

    actorId ??= new(Id.Value);
    Raise(new ApiKeyAuthenticated(), actorId.Value);
  }
  protected virtual void Handle(ApiKeyAuthenticated @event)
  {
    AuthenticatedOn = @event.OccurredOn;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ApiKeyDeleted(), actorId);
    }
  }

  public bool HasRole(Role role) => HasRole(role.Id);
  public bool HasRole(RoleId roleId) => _roles.Contains(roleId);

  public bool IsExpired(DateTime? moment = null) => ExpiresOn.HasValue && ExpiresOn.Value.AsUniversalTime() <= (moment?.AsUniversalTime() ?? DateTime.UtcNow);

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updated.CustomAttributes[key] = null;
    }
  }

  public void RemoveRole(Role role, ActorId? actorId = null)
  {
    if (HasRole(role))
    {
      Raise(new ApiKeyRoleRemoved(role.Id), actorId);
    }
  }
  public void RemoveRole(RoleId roleId, ActorId? actorId = null)
  {
    if (HasRole(roleId))
    {
      Raise(new ApiKeyRoleRemoved(roleId), actorId);
    }
  }
  protected virtual void Handle(ApiKeyRoleRemoved @event)
  {
    _roles.Remove(@event.RoleId);
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

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new ApiKeyUpdated();
    }
  }
  protected virtual void Handle(ApiKeyUpdated @event)
  {
    if (@event.Name is not null)
    {
      _name = @event.Name;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }
    if (@event.ExpiresOn.HasValue)
    {
      _expiresOn = @event.ExpiresOn.Value;
    }

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

  public override string ToString() => $"{Name} | {base.ToString()}";
}
