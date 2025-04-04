﻿using Krakenar.Constants;
using Krakenar.Extensions;
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

    services.AddControllers();
    services.AddFeatureManagement();

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
