using Krakenar.Core;

namespace Krakenar.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarWeb(this IServiceCollection services)
  {
    services.AddControllers();

    return services.AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
