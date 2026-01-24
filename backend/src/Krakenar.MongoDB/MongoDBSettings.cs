using Logitar;
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

    settings.DatabaseName = EnvironmentHelper.GetString("MONGODB_DATABASE_NAME", settings.DatabaseName);
    settings.EnableLogging = EnvironmentHelper.GetBoolean("MONGODB_ENABLE_LOGGING", settings.EnableLogging);

    return settings;
  }
}
