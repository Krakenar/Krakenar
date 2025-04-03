using Krakenar.Core.Configurations.Events;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations;

public class Configuration : AggregateRoot
{
  public new ConfigurationId Id => new(base.Id);

  private Configuration(/*Secret secret,*/ UniqueNameSettings uniqueNameSettings, PasswordSettings passwordSettings, ActorId? actorId = null)
    : base(new ConfigurationId().StreamId)
  {
    Raise(new ConfigurationInitialized(uniqueNameSettings, passwordSettings), actorId);
  }

  public static Configuration Initialize(ActorId? actorId = null, UniqueNameSettings? uniqueNameSettings = null, PasswordSettings? passwordSettings = null)
  {
    uniqueNameSettings ??= new();
    passwordSettings ??= new();
    return new Configuration(uniqueNameSettings, passwordSettings, actorId);
  }
}
