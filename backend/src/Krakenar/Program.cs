using Krakenar.Constants;
using Krakenar.Core;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Infrastructure.Commands;
using Krakenar.Settings;
using Microsoft.FeatureManagement;

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

    await startup.ConfigureAsync(application);

    using IServiceScope scope = application.Services.CreateScope();

    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(Features.MigrateDatabase))
    {
      MigrateDatabase migrateDatabase = new();
      ICommandHandler<MigrateDatabase> migrationHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<MigrateDatabase>>();
      await migrationHandler.HandleAsync(migrateDatabase);
    }

    DefaultSettings defaults = DefaultSettings.Initialize(configuration);
    InitializeConfiguration initializeConfiguration = new(defaults.Locale, defaults.UniqueName, defaults.Password);
    ICommandHandler<InitializeConfiguration> configurationHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<InitializeConfiguration>>();
    await configurationHandler.HandleAsync(initializeConfiguration);

    application.Run();
  }
}
