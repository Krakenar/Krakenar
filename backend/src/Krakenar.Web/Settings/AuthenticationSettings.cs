using Krakenar.Core;

namespace Krakenar.Web.Settings;

public record AuthenticationSettings
{
  public const string SectionKey = "Authentication";

  public AccessTokenSettings AccessToken { get; set; } = new();
  public bool EnableBasic { get; set; }

  public static AuthenticationSettings Initialize(IConfiguration configuration)
  {
    AuthenticationSettings settings = configuration.GetSection(SectionKey).Get<AuthenticationSettings>() ?? new();

    settings.AccessToken.Type = EnvironmentHelper.GetString("AUTHENTICATION_ACCESS_TOKEN_TYPE", settings.AccessToken.Type);

    string? lifetimeSecondsValue = Environment.GetEnvironmentVariable("AUTHENTICATION_ACCESS_TOKEN_LIFETIME_SECONDS");
    if (!string.IsNullOrWhiteSpace(lifetimeSecondsValue) && int.TryParse(lifetimeSecondsValue, out int lifetimeSeconds))
    {
      settings.AccessToken.LifetimeSeconds = lifetimeSeconds;
    }

    string? enableBasicValue = Environment.GetEnvironmentVariable("AUTHENTICATION_ENABLE_BASIC");
    if (!string.IsNullOrWhiteSpace(enableBasicValue) && bool.TryParse(enableBasicValue, out bool enableBasic))
    {
      settings.EnableBasic = enableBasic;
    }

    return settings;
  }
}
