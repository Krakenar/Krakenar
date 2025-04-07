using Krakenar.Core.Actors;
using Krakenar.Core.Configurations;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.EntityFrameworkCore.Relational.Actors;
using Krakenar.EntityFrameworkCore.Relational.Queriers;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddKrakenarQueriers()
      .AddScoped<IActorService, ActorService>();
  }

  public static IServiceCollection AddKrakenarQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationQuerier, ConfigurationQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>()
      .AddScoped<IRoleQuerier, RoleQuerier>();
  }
}
