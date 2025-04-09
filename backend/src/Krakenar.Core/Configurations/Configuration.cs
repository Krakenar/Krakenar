using Krakenar.Core.Configurations.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations;

public class Configuration : AggregateRoot
{
  private ConfigurationUpdated _updated = new();
  private bool HasUpdates => _updated.Secret is not null
    || _updated.UniqueNameSettings is not null || _updated.PasswordSettings is not null || _updated.LoggingSettings is not null;

  public new ConfigurationId Id => new(base.Id);

  private Secret? _secret = null;
  public Secret Secret
  {
    get => _secret ?? throw new InvalidOperationException("The configuration has not been initialized.");
    set
    {
      if (_secret != value)
      {
        _secret = value;
        _updated.Secret = value;
      }
    }
  }

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
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
    get => _passwordSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
    set
    {
      if (_passwordSettings != value)
      {
        _passwordSettings = value;
        _updated.PasswordSettings = value;
      }
    }
  }
  private LoggingSettings? _loggingSettings = null;
  public LoggingSettings LoggingSettings
  {
    get => _loggingSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
    set
    {
      if (_loggingSettings != value)
      {
        _loggingSettings = value;
        _updated.LoggingSettings = value;
      }
    }
  }

  public Configuration() : base()
  {
  }

  public static Configuration Initialize(Secret secret, ActorId actorId)
  {
    UniqueNameSettings uniqueNameSettings = new();
    PasswordSettings passwordSettings = new();
    LoggingSettings loggingSettings = new();
    ConfigurationId configurationId = new();
    return new Configuration(secret, uniqueNameSettings, passwordSettings, loggingSettings, actorId, configurationId);
  }
  private Configuration(Secret secret, UniqueNameSettings uniqueNameSettings, PasswordSettings passwordSettings, LoggingSettings loggingSettings, ActorId actorId, ConfigurationId configurationId)
    : base(configurationId.StreamId)
  {
    Raise(new ConfigurationInitialized(secret, uniqueNameSettings, passwordSettings, loggingSettings), actorId);
  }
  protected virtual void Handle(ConfigurationInitialized @event)
  {
    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _loggingSettings = @event.LoggingSettings;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new ConfigurationUpdated();
    }
  }
  protected virtual void Handle(ConfigurationUpdated @event)
  {
    if (@event.Secret is not null)
    {
      _secret = @event.Secret;
    }

    if (@event.UniqueNameSettings is not null)
    {
      _uniqueNameSettings = @event.UniqueNameSettings;
    }
    if (@event.PasswordSettings is not null)
    {
      _passwordSettings = @event.PasswordSettings;
    }
    if (@event.LoggingSettings is not null)
    {
      _loggingSettings = @event.LoggingSettings;
    }
  }
}
