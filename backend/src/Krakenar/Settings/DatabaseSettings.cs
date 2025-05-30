using Krakenar.Infrastructure;

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

    string? applyMigrationsValue = Environment.GetEnvironmentVariable("DATABASE_APPLY_MIGRATIONS");
    if (!string.IsNullOrWhiteSpace(applyMigrationsValue) && bool.TryParse(applyMigrationsValue, out bool applyMigrations))
    {
      settings.ApplyMigrations = applyMigrations;
    }

    string? providerValue = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
    if (!string.IsNullOrWhiteSpace(providerValue) && Enum.TryParse(providerValue, out DatabaseProvider provider))
    {
      settings.Provider = provider;
    }

    string? enableLoggingValue = Environment.GetEnvironmentVariable("DATABASE_ENABLE_LOGGING");
    if (!string.IsNullOrWhiteSpace(enableLoggingValue) && bool.TryParse(enableLoggingValue, out bool enableLogging))
    {
      settings.EnableLogging = enableLogging;
    }

    return settings;
  }
}
