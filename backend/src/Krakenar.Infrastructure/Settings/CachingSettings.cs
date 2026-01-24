using Logitar;
using Microsoft.Extensions.Configuration;

namespace Krakenar.Infrastructure.Settings;

public record CachingSettings
{
  public const string SectionKey = "Caching";

  public TimeSpan ActorLifetime { get; set; } = TimeSpan.FromMinutes(15);

  public static CachingSettings Initialize(IConfiguration configuration)
  {
    CachingSettings settings = configuration.GetSection(SectionKey).Get<CachingSettings>() ?? new();

    settings.ActorLifetime = EnvironmentHelper.GetTimeSpan("CACHING_ACTOR_LIFETIME", settings.ActorLifetime);

    return settings;
  }
}
