using Logitar;
using Microsoft.Extensions.Configuration;

namespace Krakenar.EntityFrameworkCore.Relational.Settings;

public record AuthenticationSettings
{
  public const string SectionKey = "Authentication";

  public bool EnableAuthenticatedEventSourcing { get; set; }

  public static AuthenticationSettings Initialize(IConfiguration configuration)
  {
    AuthenticationSettings settings = configuration.GetSection(SectionKey).Get<AuthenticationSettings>() ?? new();

    settings.EnableAuthenticatedEventSourcing = EnvironmentHelper.GetBoolean("AUTHENTICATION_ENABLE_AUTHENTICATED_EVENT_SOURCING", settings.EnableAuthenticatedEventSourcing);

    return settings;
  }
}
