using Krakenar.Core.Actors;
using Krakenar.Core.Configurations;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.Core.Sessions;
using Krakenar.Core.Users;
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
      .AddScoped<ILanguageQuerier, LanguageQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>()
      .AddScoped<IRoleQuerier, RoleQuerier>()
      .AddScoped<ISessionQuerier, SessionQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }
}
