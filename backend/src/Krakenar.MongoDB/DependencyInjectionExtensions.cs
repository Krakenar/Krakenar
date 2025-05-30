using Krakenar.Core;
using Krakenar.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Krakenar.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.TryGetString("MONGOCONNSTR_Krakenar", configuration.GetConnectionString("MongoDB"));
    MongoDBSettings settings = MongoDBSettings.Initialize(configuration);
    return services.AddKrakenarMongoDB(connectionString, settings);
  }
  public static IServiceCollection AddKrakenarMongoDB(this IServiceCollection services, string? connectionString, MongoDBSettings settings)
  {
    services.AddSingleton(settings);

    if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(settings.DatabaseName))
    {
      MongoClient client = new(connectionString.Trim());
      IMongoDatabase database = client.GetDatabase(settings.DatabaseName.Trim());
      services.AddSingleton(database);

      if (settings.EnableLogging)
      {
        services.AddScoped<ILogRepository, LogRepository>();
      }
    }

    return services;
  }
}
