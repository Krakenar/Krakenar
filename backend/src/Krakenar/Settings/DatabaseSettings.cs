namespace Krakenar.Settings;

internal record DatabaseSettings
{
  public const string SectionKey = "Database";

  public bool ApplyMigrations { get; set; }

  public static DatabaseSettings Initialize(IConfiguration configuration)
  {
    DatabaseSettings settings = configuration.GetSection(SectionKey).Get<DatabaseSettings>() ?? new();

    string? applyMigrationsValue = Environment.GetEnvironmentVariable("DATABASE_APPLY_MIGRATIONS");
    if (!string.IsNullOrWhiteSpace(applyMigrationsValue) && bool.TryParse(applyMigrationsValue, out bool applyMigrations))
    {
      settings.ApplyMigrations = applyMigrations;
    }

    return settings;
  }
}
