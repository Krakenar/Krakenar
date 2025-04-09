using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Configurations;
using Krakenar.Core.Configurations.Events;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Events;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Events;
using Krakenar.Core.Sessions;
using Krakenar.Core.Users;
using Krakenar.EntityFrameworkCore.Relational.Actors;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Krakenar.EntityFrameworkCore.Relational.Queriers;
using Krakenar.Infrastructure.Commands;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddKrakenarQueriers()
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddScoped<IActorService, ActorService>()
      .AddScoped<ICommandHandler<MigrateDatabase>, MigrateDatabaseCommandHandler>();
  }

  public static IServiceCollection AddKrakenarEventHandlers(this IServiceCollection services)
  {
    return services
      .AddScoped<IEventHandler<ConfigurationInitialized>, ConfigurationEvents>()
      .AddScoped<IEventHandler<ConfigurationUpdated>, ConfigurationEvents>()
      .AddScoped<IEventHandler<LanguageCreated>, LanguageEvents>()
      .AddScoped<IEventHandler<LanguageDeleted>, LanguageEvents>()
      .AddScoped<IEventHandler<LanguageLocaleChanged>, LanguageEvents>()
      .AddScoped<IEventHandler<LanguageSetDefault>, LanguageEvents>()
      .AddScoped<IEventHandler<RealmCreated>, RealmEvents>()
      .AddScoped<IEventHandler<RealmDeleted>, RealmEvents>()
      .AddScoped<IEventHandler<RealmUniqueSlugChanged>, RealmEvents>()
      .AddScoped<IEventHandler<RealmUpdated>, RealmEvents>()
      .AddScoped<IEventHandler<RoleCreated>, RoleEvents>()
      .AddScoped<IEventHandler<RoleDeleted>, RoleEvents>()
      .AddScoped<IEventHandler<RoleUniqueNameChanged>, RoleEvents>()
      .AddScoped<IEventHandler<RoleUpdated>, RoleEvents>();
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
