namespace Krakenar.Web.Settings;

public record ErrorSettings
{
  public const string SectionKey = "Error";

  public bool ExposeDetail { get; set; }

  public static ErrorSettings Initialize(IConfiguration configuration)
  {
    ErrorSettings settings = configuration.GetSection(SectionKey).Get<ErrorSettings>() ?? new();

    string? exposeDetailValue = Environment.GetEnvironmentVariable("ERROR_EXPOSE_DETAIL");
    if (!string.IsNullOrWhiteSpace(exposeDetailValue) && bool.TryParse(exposeDetailValue, out bool exposeDetail))
    {
      settings.ExposeDetail = exposeDetail;
    }

    return settings;
  }
}
