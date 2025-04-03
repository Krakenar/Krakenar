namespace Krakenar.Settings;

internal record DefaultSettings
{
  public string Locale { get; set; } = "en";
  public string UniqueName { get; set; } = "admin";
  public string Password { get; set; } = "P@s$W0rD";

  public static DefaultSettings Initialize(IConfiguration configuration)
  {
    DefaultSettings settings = configuration.GetSection("Default").Get<DefaultSettings>() ?? new();

    //settings.Locale = EnvironmentHelper.GetString("DEFAULT_LOCALE", settings.Locale);
    //settings.Locale = EnvironmentHelper.GetString("DEFAULT_USERNAME", settings.UniqueName);
    //settings.Locale = EnvironmentHelper.GetString("DEFAULT_PASSWORD", settings.Password);

    return settings;
  }
}
