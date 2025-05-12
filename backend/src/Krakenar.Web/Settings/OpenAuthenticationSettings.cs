using Krakenar.Core;

namespace Krakenar.Web.Settings;

public record OpenAuthenticationSettings
{
  public const string SectionKey = "OpenAuthentication";

  public AccessTokenSettings AccessToken { get; set; } = new();

  public static OpenAuthenticationSettings Initialize(IConfiguration configuration)
  {
    OpenAuthenticationSettings settings = configuration.GetSection(SectionKey).Get<OpenAuthenticationSettings>() ?? new();

    settings.AccessToken.Type = EnvironmentHelper.GetString("OPEN_AUTHENTICATION_ACCESS_TOKEN_TYOE", settings.AccessToken.Type);

    string? lifetimeSecondsValue = Environment.GetEnvironmentVariable("OPEN_AUTHENTICATION_ACCESS_TOKEN_LIFETIME_SECONDS");
    if (!string.IsNullOrWhiteSpace(lifetimeSecondsValue) && int.TryParse(lifetimeSecondsValue, out int lifetimeSeconds))
    {
      settings.AccessToken.LifetimeSeconds = lifetimeSeconds;
    }

    return settings;
  }
}
