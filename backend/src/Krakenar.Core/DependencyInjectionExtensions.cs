using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarCore(this IServiceCollection services)
  {
    return services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }
}
