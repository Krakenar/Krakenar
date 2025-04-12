using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Configurations.Events;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ConfigurationEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Configuration;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class ConfigurationEvents : IEventHandler<ConfigurationInitialized>, IEventHandler<ConfigurationUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<ConfigurationEvents> Logger { get; }

  public ConfigurationEvents(KrakenarContext context, ILogger<ConfigurationEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(ConfigurationInitialized @event, CancellationToken cancellationToken)
  {
    Dictionary<string, ConfigurationEntity> configurations = await Context.Configuration.ToDictionaryAsync(x => x.Key, x => x, cancellationToken);

    SetSecret(configurations, @event.Secret, @event);
    SetUniqueNameSettings(configurations, @event.UniqueNameSettings, @event);
    SetPasswordSettings(configurations, @event.PasswordSettings, @event);
    SetLoggingSettings(configurations, @event.LoggingSettings, @event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ConfigurationUpdated @event, CancellationToken cancellationToken)
  {
    Dictionary<string, ConfigurationEntity> configurations = await Context.Configuration.ToDictionaryAsync(x => x.Key, x => x, cancellationToken);

    if (@event.Secret is not null)
    {
      SetSecret(configurations, @event.Secret, @event);
    }
    if (@event.UniqueNameSettings is not null)
    {
      SetUniqueNameSettings(configurations, @event.UniqueNameSettings, @event);
    }
    if (@event.PasswordSettings is not null)
    {
      SetPasswordSettings(configurations, @event.PasswordSettings, @event);
    }
    if (@event.LoggingSettings is not null)
    {
      SetLoggingSettings(configurations, @event.LoggingSettings, @event);
    }

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  private void SetSecret(IDictionary<string, ConfigurationEntity> configurations, Secret secret, DomainEvent @event)
  {
    SetConfiguration(configurations, "Secret", secret, @event);
  }

  private void SetLoggingSettings(IDictionary<string, ConfigurationEntity> configurations, ILoggingSettings loggingSettings, DomainEvent @event)
  {
    SetConfiguration(configurations, "LoggingSettings.Extent", loggingSettings.Extent, @event);
    SetConfiguration(configurations, "LoggingSettings.OnlyErrors", loggingSettings.OnlyErrors, @event);
  }

  private void SetPasswordSettings(IDictionary<string, ConfigurationEntity> configurations, IPasswordSettings passwordSettings, DomainEvent @event)
  {
    SetConfiguration(configurations, "PasswordSettings.RequiredLength", passwordSettings.RequiredLength, @event);
    SetConfiguration(configurations, "PasswordSettings.RequiredUniqueChars", passwordSettings.RequiredUniqueChars, @event);
    SetConfiguration(configurations, "PasswordSettings.RequireNonAlphanumeric", passwordSettings.RequireNonAlphanumeric, @event);
    SetConfiguration(configurations, "PasswordSettings.RequireLowercase", passwordSettings.RequireLowercase, @event);
    SetConfiguration(configurations, "PasswordSettings.RequireUppercase", passwordSettings.RequireUppercase, @event);
    SetConfiguration(configurations, "PasswordSettings.RequireDigit", passwordSettings.RequireDigit, @event);
    SetConfiguration(configurations, "PasswordSettings.HashingStrategy", passwordSettings.HashingStrategy, @event);
  }

  private void SetUniqueNameSettings(IDictionary<string, ConfigurationEntity> configurations, IUniqueNameSettings uniqueNameSettings, DomainEvent @event)
  {
    if (uniqueNameSettings.AllowedCharacters is null)
    {
      if (configurations.TryGetValue("UniqueNameSettings.AllowedCharacters", out ConfigurationEntity? configuration))
      {
        Context.Configuration.Remove(configuration);
      }
    }
    else
    {
      SetConfiguration(configurations, "UniqueNameSettings.AllowedCharacters", uniqueNameSettings.AllowedCharacters, @event);
    }
  }

  private void SetConfiguration(IDictionary<string, ConfigurationEntity> configurations, string key, object value, DomainEvent @event)
  {
    if (configurations.TryGetValue(key, out ConfigurationEntity? configuration))
    {
      configuration.Update(value, @event);
    }
    else
    {
      configuration = new ConfigurationEntity(key, value, @event);
      configurations[key] = configuration;

      Context.Configuration.Add(configuration);
    }
  }
}
