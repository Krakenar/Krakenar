using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarEntityFrameworkCorePostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Krakenar", configuration.GetConnectionString("PostgreSQL"));
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The connection string for the database provider '{DatabaseProvider.EntityFrameworkCorePostgreSQL}' could not be found.", nameof(configuration));
    }
    return services.AddKrakenarEntityFrameworkCorePostgreSQL(connectionString.Trim());
  }
  public static IServiceCollection AddKrakenarEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<KrakenarContext>(options => options.UseNpgsql(connectionString, options => options.MigrationsAssembly("Krakenar.EntityFrameworkCore.PostgreSQL")))
      .AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddSingleton<ISqlHelper, PostgresHelper>();
  }
}
