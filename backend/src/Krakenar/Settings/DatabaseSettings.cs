using Krakenar.Infrastructure;
using Logitar;

namespace Krakenar.Settings;

internal record DatabaseSettings
{
  public const string SectionKey = "Database";

  public bool ApplyMigrations { get; set; }
  public DatabaseProvider Provider { get; set; } = DatabaseProvider.EntityFrameworkCoreSqlServer;
  public bool EnableLogging { get; set; }

  public static DatabaseSettings Initialize(IConfiguration configuration)
  {
    DatabaseSettings settings = configuration.GetSection(SectionKey).Get<DatabaseSettings>() ?? new();

    settings.ApplyMigrations = EnvironmentHelper.GetBoolean("DATABASE_APPLY_MIGRATIONS", settings.ApplyMigrations);
    settings.Provider = EnvironmentHelper.GetEnum("DATABASE_PROVIDER", settings.Provider);
    settings.EnableLogging = EnvironmentHelper.GetBoolean("DATABASE_ENABLE_LOGGING", settings.EnableLogging);

    return settings;
  }
}
