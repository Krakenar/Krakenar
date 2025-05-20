using Krakenar.Core;

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

    string? enableSwaggerValue = Environment.GetEnvironmentVariable("ADMIN_ENABLE_SWAGGER");
    if (!string.IsNullOrWhiteSpace(enableSwaggerValue) && bool.TryParse(enableSwaggerValue, out bool enableSwagger))
    {
      settings.EnableSwagger = enableSwagger;
    }

    settings.Title = EnvironmentHelper.GetString("ADMIN_TITLE", settings.Title);

    string? versionValue = Environment.GetEnvironmentVariable("ADMIN_VERSION");
    if (!string.IsNullOrWhiteSpace(versionValue) && Version.TryParse(versionValue, out Version? version))
    {
      settings.Version = version;
    }

    return settings;
  }
}
