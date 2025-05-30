using Microsoft.Extensions.Configuration;

namespace Krakenar.MongoDB;

public record MongoDBSettings
{
  public const string SectionKey = "MongoDB";

  public string DatabaseName { get; set; } = string.Empty;
  public bool EnableLogging { get; set; }

  public static MongoDBSettings Initialize(IConfiguration configuration)
  {
    MongoDBSettings settings = configuration.GetSection(SectionKey).Get<MongoDBSettings>() ?? new();

    string? databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
    if (!string.IsNullOrWhiteSpace(databaseName))
    {
      settings.DatabaseName = databaseName.Trim();
    }

    string? enableLoggingValue = Environment.GetEnvironmentVariable("MONGODB_ENABLE_LOGGING");
    if (!string.IsNullOrWhiteSpace(enableLoggingValue) && bool.TryParse(enableLoggingValue, out bool enableLogging))
    {
      settings.EnableLogging = enableLogging;
    }

    return settings;
  }
}
