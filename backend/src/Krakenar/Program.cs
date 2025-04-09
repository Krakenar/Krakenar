using Krakenar.Core;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Settings;

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

    DefaultSettings defaults = DefaultSettings.Initialize(configuration);
    InitializeConfiguration initializeConfiguration = new(defaults.Locale, defaults.UniqueName, defaults.Password);
    ICommandHandler<InitializeConfiguration> handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<InitializeConfiguration>>();
    await handler.HandleAsync(initializeConfiguration);

    application.Run();
  }
}
