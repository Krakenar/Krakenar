using Krakenar.Constants;
using Krakenar.Core;
using Krakenar.Extensions;
using Krakenar.Infrastructure;
using Krakenar.Web;
using Microsoft.FeatureManagement;

namespace Krakenar;

internal class Startup : StartupBase
{
  public Startup(IConfiguration _)
  {
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddFeatureManagement();

    services.AddKrakenarCore();
    services.AddKrakenarInfrastructure();
    services.AddKrakenarWeb();
    services.AddKrakenarSwagger();
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
  }
}
