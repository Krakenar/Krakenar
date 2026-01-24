using Logitar;

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
    settings.AccessToken.LifetimeSeconds = EnvironmentHelper.GetInt32("AUTHENTICATION_ACCESS_TOKEN_LIFETIME_SECONDS", settings.AccessToken.LifetimeSeconds);
    settings.EnableBasic = EnvironmentHelper.GetBoolean("AUTHENTICATION_ENABLE_BASIC", settings.EnableBasic);

    return settings;
  }
}
