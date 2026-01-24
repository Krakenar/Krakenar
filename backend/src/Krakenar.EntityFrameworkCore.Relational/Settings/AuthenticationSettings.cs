using Logitar;
using Microsoft.Extensions.Configuration;

namespace Krakenar.EntityFrameworkCore.Relational.Settings;

public record AuthenticationSettings
{
  public const string SectionKey = "Authentication";

  public bool SilentAuthenticatedEvent { get; set; } // TODO(fpion): should it be true by default?

  public static AuthenticationSettings Initialize(IConfiguration configuration)
  {
    AuthenticationSettings settings = configuration.GetSection(SectionKey).Get<AuthenticationSettings>() ?? new();

    settings.SilentAuthenticatedEvent = EnvironmentHelper.GetBoolean("AUTHENTICATION_SILENT_AUTHENTICATED_EVENT", settings.SilentAuthenticatedEvent);

    return settings;
  }
}
