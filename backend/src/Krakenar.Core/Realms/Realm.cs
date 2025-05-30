using Krakenar.Core.Realms.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;

namespace Krakenar.Core.Realms;

public class Realm : AggregateRoot, ICustomizable
{
  private RealmUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null
    || _updated.Url is not null
    || _updated.UniqueNameSettings is not null || _updated.PasswordSettings is not null || _updated.RequireUniqueEmail is not null || _updated.RequireConfirmedAccount is not null
    || _updated.CustomAttributes.Count > 0;

  public new RealmId Id => new(base.Id);

  private Slug? _uniqueSlug = null;
  public Slug UniqueSlug => _uniqueSlug ?? throw new InvalidOperationException("The realm has not been initialized.");
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
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

  private Secret? _secret = null;
  public Secret Secret => _secret ?? throw new InvalidOperationException("The realm has not been initialized.");

  private Url? _url = null;
  public Url? Url
  {
    get => _url;
    set
    {
      if (_url != value)
      {
        _url = value;
        _updated.Url = new Change<Url>(value);
      }
    }
  }

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings ?? throw new InvalidOperationException("The realm has not been initialized.");
    set
    {
      if (_uniqueNameSettings != value)
      {
        _uniqueNameSettings = value;
        _updated.UniqueNameSettings = value;
      }
    }
  }
  private PasswordSettings? _passwordSettings = null;
  public PasswordSettings PasswordSettings
  {
    get => _passwordSettings ?? throw new InvalidOperationException("The realm has not been initialized.");
    set
    {
      if (_passwordSettings != value)
      {
        _passwordSettings = value;
        _updated.PasswordSettings = value;
      }
    }
  }
  private bool _requireUniqueEmail = false;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail;
    set
    {
      if (_requireUniqueEmail != value)
      {
        _requireUniqueEmail = value;
        _updated.RequireUniqueEmail = value;
      }
    }
  }
  private bool _requireConfirmedAccount = false;
  public bool RequireConfirmedAccount
  {
    get => _requireConfirmedAccount;
    set
    {
      if (_requireConfirmedAccount != value)
      {
        _requireConfirmedAccount = value;
        _updated.RequireConfirmedAccount = value;
      }
    }
  }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  public Realm() : base()
  {
  }

  public Realm(Slug uniqueSlug, Secret secret, ActorId? actorId = null, RealmId? realmId = null)
    : base((realmId ?? RealmId.NewId()).StreamId)
  {
    UniqueNameSettings uniqueNameSettings = new UniqueNameSettings();
    PasswordSettings passwordSettings = new();
    bool requireUniqueEmail = true;
    bool requireConfirmedAccount = true;
    Raise(new RealmCreated(uniqueSlug, secret, uniqueNameSettings, passwordSettings, requireUniqueEmail, requireConfirmedAccount), actorId);
  }
  protected virtual void Handle(RealmCreated @event)
  {
    _uniqueSlug = @event.UniqueSlug;

    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;
    _requireConfirmedAccount = @event.RequireConfirmedAccount;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new RealmDeleted(), actorId);
    }
  }

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updated.CustomAttributes[key] = null;
    }
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

  public void SetSecret(Secret secret, ActorId? actorId = null)
  {
    if (_secret != secret)
    {
      Raise(new RealmSecretChanged(secret), actorId);
    }
  }
  protected virtual void Handle(RealmSecretChanged @event)
  {
    _secret = @event.Secret;
  }

  public void SetUniqueSlug(Slug uniqueSlug, ActorId? actorId = null)
  {
    if (_uniqueSlug != uniqueSlug)
    {
      Raise(new RealmUniqueSlugChanged(uniqueSlug), actorId);
    }
  }
  protected virtual void Handle(RealmUniqueSlugChanged @event)
  {
    _uniqueSlug = @event.UniqueSlug;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new RealmUpdated();
    }
  }
  protected virtual void Handle(RealmUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Url is not null)
    {
      _url = @event.Url.Value;
    }

    if (@event.UniqueNameSettings is not null)
    {
      _uniqueNameSettings = @event.UniqueNameSettings;
    }
    if (@event.PasswordSettings is not null)
    {
      _passwordSettings = @event.PasswordSettings;
    }
    if (@event.RequireUniqueEmail.HasValue)
    {
      _requireUniqueEmail = @event.RequireUniqueEmail.Value;
    }
    if (@event.RequireConfirmedAccount.HasValue)
    {
      _requireConfirmedAccount = @event.RequireConfirmedAccount.Value;
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

  public override string ToString() => $"{DisplayName?.Value ?? UniqueSlug.Value} | {base.ToString()}";
}
