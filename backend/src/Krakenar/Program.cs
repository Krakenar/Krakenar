using Krakenar.Core.Configurations.Commands;
using Krakenar.Infrastructure.Commands;
using Krakenar.Settings;
using Logitar.CQRS;

namespace Krakenar;

internal class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();

    DatabaseSettings database = application.Services.GetRequiredService<DatabaseSettings>();
    if (database.ApplyMigrations)
    {
      MigrateDatabase migrateDatabase = new();
      ICommandHandler<MigrateDatabase, Unit> migrationHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<MigrateDatabase, Unit>>();
      await migrationHandler.HandleAsync(migrateDatabase);
    }

    DefaultSettings defaults = DefaultSettings.Initialize(configuration);
    InitializeConfiguration initializeConfiguration = new(defaults.Locale, defaults.UniqueName, defaults.Password);
    ICommandHandler<InitializeConfiguration, Unit> configurationHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<InitializeConfiguration, Unit>>();
    await configurationHandler.HandleAsync(initializeConfiguration);

    application.Run();
  }
}
