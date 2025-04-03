using Krakenar.Core.Configurations.Events;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations;

public class Configuration : AggregateRoot
{
  public new ConfigurationId Id => new(base.Id);

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings => _uniqueNameSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
  private PasswordSettings? _passwordSettings = null;
  public PasswordSettings PasswordSettings => _passwordSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");

  private Configuration(/*Secret secret,*/ UniqueNameSettings uniqueNameSettings, PasswordSettings passwordSettings, ActorId? actorId = null)
    : base(new ConfigurationId().StreamId)
  {
    Raise(new ConfigurationInitialized(uniqueNameSettings, passwordSettings), actorId);
  }
  protected virtual void Handle(ConfigurationInitialized @event)
  {
    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
  }

  public static Configuration Initialize(ActorId? actorId = null, UniqueNameSettings? uniqueNameSettings = null, PasswordSettings? passwordSettings = null)
  {
    uniqueNameSettings ??= new();
    passwordSettings ??= new();
    return new Configuration(uniqueNameSettings, passwordSettings, actorId);
  }
}
