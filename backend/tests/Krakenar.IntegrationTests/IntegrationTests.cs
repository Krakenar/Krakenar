using Krakenar.Core;
using Krakenar.Core.Configurations.Commands;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.SqlServer;
using Krakenar.Infrastructure;
using Krakenar.Infrastructure.Commands;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KrakenarDb = Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

namespace Krakenar;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;

  protected IServiceProvider ServiceProvider { get; }
  protected KrakenarContext KrakenarContext { get; }

  protected IntegrationTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    _databaseProvider = configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;

    ServiceCollection services = new();
    services.AddSingleton(configuration);

    services.AddKrakenarCore();
    services.AddKrakenarInfrastructure();
    services.AddKrakenarEntityFrameworkCoreRelational();

    string? connectionString;
    switch (_databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = EnvironmentHelper.TryGetString("SQLCONNSTR_Krakenar", configuration.GetConnectionString("SqlServer"))?.Replace("{Database}", GetType().Name)
          ?? throw new InvalidOperationException($"The connection string for the database provider '{DatabaseProvider.EntityFrameworkCoreSqlServer}' could not be found.");
        services.AddKrakenarEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(_databaseProvider);
    }

    services.AddSingleton<IApplicationContext>(new TestApplicationContext());

    ServiceProvider = services.BuildServiceProvider();

    KrakenarContext = ServiceProvider.GetRequiredService<KrakenarContext>();
  }

  public virtual async Task InitializeAsync()
  {
    MigrateDatabase command = new();
    ICommandHandler<MigrateDatabase> migrationHandler = ServiceProvider.GetRequiredService<ICommandHandler<MigrateDatabase>>();
    await migrationHandler.HandleAsync(command);

    StringBuilder sql = new();
    TableId[] tables =
    [
      KrakenarDb.Languages.Table,
      KrakenarDb.Users.Table,
      KrakenarDb.Configuration.Table,
      KrakenarDb.Actors.Table,
      EventDb.Streams.Table
    ];
    foreach (TableId table in tables)
    {
      Logitar.Data.ICommand delete = CreateDeleteBuilder(table).Build();
      sql.AppendLine(delete.Text);
    }
    await KrakenarContext.Database.ExecuteSqlRawAsync(sql.ToString());

    InitializeConfiguration initializeConfiguration = new("en", "admin", "P@s$W0rD");
    ICommandHandler<InitializeConfiguration> configurationHandler = ServiceProvider.GetRequiredService<ICommandHandler<InitializeConfiguration>>();
    await configurationHandler.HandleAsync(initializeConfiguration);
  }
  private IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.EntityFrameworkCoreSqlServer => new SqlServerDeleteBuilder(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public Task DisposeAsync() => Task.CompletedTask;
}
