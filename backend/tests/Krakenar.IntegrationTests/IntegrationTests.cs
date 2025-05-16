using Bogus;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Caching;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Krakenar.Core.Users;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.SqlServer;
using Krakenar.Infrastructure;
using Krakenar.Infrastructure.Commands;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ActorDto = Krakenar.Contracts.Actors.Actor;
using KrakenarDb = Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;

  protected Faker Faker { get; } = new();

  private readonly TestApplicationContext _applicationContext = new();
  protected ActorDto Actor { get; private set; } = new();
  protected ActorId? ActorId => _applicationContext.ActorId;
  protected Realm Realm { get; }
  protected RealmDto? RealmDto => _applicationContext.Realm;

  protected IServiceProvider ServiceProvider { get; }
  protected KrakenarContext KrakenarContext { get; }

  protected IntegrationTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("9d4b585d-9a96-4cb3-b1ff-7a8d77eda766")
      .Build();

    _databaseProvider = configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;

    ServiceCollection services = new();
    services.AddLogging();
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

    services.AddSingleton<IApplicationContext>(_applicationContext);

    ServiceProvider = services.BuildServiceProvider();

    KrakenarContext = ServiceProvider.GetRequiredService<KrakenarContext>();

    RealmId realmId = RealmId.NewId();
    ISecretManager secretManager = ServiceProvider.GetRequiredService<ISecretManager>();
    Secret secret = secretManager.Generate(realmId);
    Realm = new Realm(new Slug("kraken"), secret, actorId: null, realmId)
    {
      DisplayName = new DisplayName("Kraken"),
      Description = new Description("This is the realm of the kraken!")
    };
    Realm.Update();
  }

  public virtual async Task InitializeAsync()
  {
    MigrateDatabase command = new();
    ICommandHandler<MigrateDatabase> migrationHandler = ServiceProvider.GetRequiredService<ICommandHandler<MigrateDatabase>>();
    await migrationHandler.HandleAsync(command);

    StringBuilder sql = new();
    TableId[] tables =
    [
      KrakenarDb.Contents.Table,
      KrakenarDb.ContentTypes.Table,
      KrakenarDb.FieldTypes.Table,
      KrakenarDb.Messages.Table,
      KrakenarDb.Templates.Table,
      KrakenarDb.Senders.Table,
      KrakenarDb.Dictionaries.Table,
      KrakenarDb.Languages.Table,
      KrakenarDb.TokenBlacklist.Table,
      KrakenarDb.Sessions.Table,
      KrakenarDb.OneTimePasswords.Table,
      KrakenarDb.Users.Table,
      KrakenarDb.ApiKeys.Table,
      KrakenarDb.Roles.Table,
      KrakenarDb.Actors.Table,
      KrakenarDb.Realms.Table,
      KrakenarDb.Configuration.Table,
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

    ICacheService cacheService = ServiceProvider.GetRequiredService<ICacheService>();
    _applicationContext.Configuration = cacheService.Configuration;
    Assert.NotNull(_applicationContext.Configuration);

    IUserQuerier userQuerier = ServiceProvider.GetRequiredService<IUserQuerier>();
    UserDto? user = await userQuerier.ReadAsync(initializeConfiguration.UniqueName);
    Assert.NotNull(user);
    Actor = new ActorDto(user);
    _applicationContext.ActorId = Actor.GetActorId();

    IRealmRepository realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();
    await realmRepository.SaveAsync(Realm);
    IRealmQuerier realmQuerier = ServiceProvider.GetRequiredService<IRealmQuerier>();
    _applicationContext.Realm = await realmQuerier.ReadAsync(Realm);
    _applicationContext.RealmId = Realm.Id;

    ILanguageRepository languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    Language language = new(new Locale(initializeConfiguration.DefaultLocale), isDefault: true, ActorId, LanguageId.NewId(Realm.Id));
    await languageRepository.SaveAsync(language);
  }
  private IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.EntityFrameworkCoreSqlServer => new SqlServerDeleteBuilder(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public Task DisposeAsync() => Task.CompletedTask;
}
