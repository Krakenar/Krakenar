using Krakenar.Constants;
using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.SqlServer;
using Krakenar.Extensions;
using Krakenar.Infrastructure;
using Krakenar.Web;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.FeatureManagement;

namespace Krakenar;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddApplicationInsightsTelemetry();
    services.AddFeatureManagement();

    services.AddKrakenarCore();
    services.AddKrakenarInfrastructure();
    services.AddKrakenarEntityFrameworkCoreRelational();
    services.AddKrakenarWeb();
    services.AddKrakenarSwagger();

    IHealthChecksBuilder healthChecks = services.AddHealthChecks();
    DatabaseProvider databaseProvider = GetDatabaseProvider();
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddKrakenarEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<KrakenarContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }
  private DatabaseProvider GetDatabaseProvider()
  {
    string? value = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
    if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, out DatabaseProvider databaseProvider) && Enum.IsDefined(databaseProvider))
    {
      return databaseProvider;
    }
    return _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      ConfigureAsync(application).Wait();
    }
  }
  public virtual async Task ConfigureAsync(WebApplication application)
  {
    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();

    if (await featureManager.IsEnabledAsync(Features.UseSwaggerUI))
    {
      application.UseKrakenarSwagger();
    }

    application.UseHttpsRedirection();

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
