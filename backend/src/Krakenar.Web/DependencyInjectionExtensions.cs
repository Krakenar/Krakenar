using Krakenar.Core;
using System.Text.Json.Serialization;

namespace Krakenar.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews()
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    return services
      .AddHttpContextAccessor()
      .AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
