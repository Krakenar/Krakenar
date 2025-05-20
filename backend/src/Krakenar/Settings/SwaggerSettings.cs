namespace Krakenar.Settings;

internal record SwaggerSettings
{
  public const string SectionKey = "Swagger";

  public bool Enabled { get; set; }

  public static SwaggerSettings Initialize(IConfiguration configuration)
  {
    SwaggerSettings settings = configuration.GetSection(SectionKey).Get<SwaggerSettings>() ?? new();

    string? enabledValue = Environment.GetEnvironmentVariable("SWAGGER_ENABLED");
    if (!string.IsNullOrWhiteSpace(enabledValue) && bool.TryParse(enabledValue, out bool enabled))
    {
      settings.Enabled = enabled;
    }

    return settings;
  }
}
