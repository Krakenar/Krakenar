using Logitar;

namespace Krakenar.Web.Settings;

public record AdminSettings
{
  public const string SectionKey = "Admin";

  public string BasePath { get; set; } = "admin";
  public bool EnableSwagger { get; set; }
  public string Title { get; set; } = "Krakenar API";
  public Version Version { get; set; } = new(0, 1, 0);

  public static AdminSettings Initialize(IConfiguration configuration)
  {
    AdminSettings settings = configuration.GetSection(SectionKey).Get<AdminSettings>() ?? new();

    settings.BasePath = EnvironmentHelper.GetString("ADMIN_BASE_PATH", settings.BasePath);
    settings.EnableSwagger = EnvironmentHelper.GetBoolean("ADMIN_ENABLE_SWAGGER", settings.EnableSwagger);
    settings.Title = EnvironmentHelper.GetString("ADMIN_TITLE", settings.Title);

    string? versionValue = EnvironmentHelper.TryGetString("ADMIN_VERSION");
    if (Version.TryParse(versionValue, out Version? version))
    {
      settings.Version = version;
    }

    return settings;
  }
}
