using Logitar;

namespace Krakenar.Settings;

internal record DefaultSettings
{
  public const string SectionKey = "Default";

  public string Locale { get; set; } = "en";
  public string UniqueName { get; set; } = "admin";
  public string Password { get; set; } = "P@s$W0rD";

  public static DefaultSettings Initialize(IConfiguration configuration)
  {
    DefaultSettings settings = configuration.GetSection(SectionKey).Get<DefaultSettings>() ?? new();

    settings.Locale = EnvironmentHelper.GetString("DEFAULT_LOCALE", settings.Locale);
    settings.UniqueName = EnvironmentHelper.GetString("DEFAULT_USERNAME", settings.UniqueName);
    settings.Password = EnvironmentHelper.GetString("DEFAULT_PASSWORD", settings.Password);

    return settings;
  }
}
