using Krakenar.Core;

namespace Krakenar.Web.Settings;

public record AdminSettings
{
  public const string SectionKey = "Admin";

  public string BasePath { get; set; } = "admin";
  public bool EnableSwagger { get; set; }

  public static AdminSettings Initialize(IConfiguration configuration)
  {
    AdminSettings settings = configuration.GetSection(SectionKey).Get<AdminSettings>() ?? new();

    settings.BasePath = EnvironmentHelper.GetString("ADMIN_BASE_PATH", settings.BasePath);

    string? enableSwaggerValue = Environment.GetEnvironmentVariable("ADMIN_ENABLE_SWAGGER");
    if (!string.IsNullOrWhiteSpace(enableSwaggerValue) && bool.TryParse(enableSwaggerValue, out bool enableSwagger))
    {
      settings.EnableSwagger = enableSwagger;
    }

    return settings;
  }
}
