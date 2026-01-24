using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure;
using Logitar;
using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.GetString("SQLCONNSTR_Krakenar") ?? configuration.GetConnectionString("SqlServer");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The connection string for the database provider '{DatabaseProvider.EntityFrameworkCoreSqlServer}' could not be found.", nameof(configuration));
    }
    return services.AddKrakenarEntityFrameworkCoreSqlServer(connectionString.Trim());
  }
  public static IServiceCollection AddKrakenarEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<KrakenarContext>(options => options.UseSqlServer(connectionString, options => options.MigrationsAssembly("Krakenar.EntityFrameworkCore.SqlServer")))
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString)
      .AddSingleton<ISqlHelper, SqlServerHelper>();
  }
}
