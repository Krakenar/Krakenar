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

    DefaultSettings defaults = DefaultSettings.Initialize(configuration);
    //InitializeConfigurationCommand command = new(defaults.Locale, defaults.UniqueName, defaults.Password);

    using IServiceScope scope = application.Services.CreateScope();
    //IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    //await mediator.Send(command);

    application.Run();
  }
}
