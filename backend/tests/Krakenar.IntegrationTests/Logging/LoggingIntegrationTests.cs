using Bogus;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Logging;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Commands;
using Krakenar.Core.Users.Events;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.SqlServer;
using Krakenar.Infrastructure;
using Krakenar.MongoDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoLog = Krakenar.MongoDB.Log;
using RelationalLog = Krakenar.EntityFrameworkCore.Relational.Entities.Log;

namespace Krakenar.Logging;

[Trait(Traits.Category, Categories.Integration)]
public class LoggingIntegrationTests : IntegrationTests
{
  private readonly Faker _faker = new();

  private readonly ILoggingService _loggingService;

  public LoggingIntegrationTests() : base()
  {
    _loggingService = ServiceProvider.GetRequiredService<ILoggingService>();
  }

  [Fact(DisplayName = "It should save the log to multiple repositories.")]
  public async Task Given_Repositories_When_Save_Then_LogsSaved()
  {
    IConfiguration configuration = ServiceProvider.GetRequiredService<IConfiguration>();
    string connectionString = EnvironmentHelper.TryGetString("SQLCONNSTR_Krakenar", configuration.GetConnectionString("SqlServer"))?.Replace("{Database}", GetType().Name)
      ?? throw new InvalidOperationException($"The connection string for the database provider '{DatabaseProvider.EntityFrameworkCoreSqlServer}' could not be found.");

    IApplicationContext context = ServiceProvider.GetRequiredService<IApplicationContext>();

    ServiceCollection services = new();
    services.AddLogging();
    services.AddSingleton(configuration);
    services.AddSingleton(context);
    services.AddKrakenarCore();
    services.AddKrakenarInfrastructure();
    services.AddKrakenarEntityFrameworkCoreRelational();
    services.AddKrakenarEntityFrameworkCoreSqlServer(connectionString);
    services.AddKrakenarMongoDB(configuration);
    services.AddScoped<ILogRepository, EntityFrameworkCore.Relational.Repositories.LogRepository>();
    IServiceProvider serviceProvider = services.BuildServiceProvider();
    ILoggingService loggingService = serviceProvider.GetRequiredService<ILoggingService>();

    string correlationId = Guid.NewGuid().ToString();
    loggingService.Open(correlationId, "GET", "api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");

    UserCreated @event = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), Password: null);
    loggingService.Report(@event);

    Exception exception = new("Invalid request.");
    loggingService.Report(exception);

    CreateOrReplaceUser command = new(Id: null, new CreateOrReplaceUserPayload(_faker.Person.UserName), Version: null);
    loggingService.SetActivity(command);

    Operation operation = new("POST", "CreateAsync");
    loggingService.SetOperation(operation);

    Realm? realm = new()
    {
      Id = Guid.NewGuid()
    };
    loggingService.SetRealm(realm);

    ApiKey? apiKey = new()
    {
      Id = Guid.NewGuid()
    };
    loggingService.SetApiKey(apiKey);

    Session? session = new()
    {
      Id = Guid.NewGuid()
    };
    loggingService.SetSession(session);

    User? user = new()
    {
      Id = Guid.NewGuid()
    };
    loggingService.SetUser(user);

    await loggingService.CloseAndSaveAsync((int)HttpStatusCode.OK);

    RelationalLog? relationalLog = await KrakenarContext.Logs
      .Include(x => x.Events)
      .Include(x => x.Exceptions)
      .SingleOrDefaultAsync();
    Assert.NotNull(relationalLog);
    Assert.Equal(correlationId, relationalLog.CorrelationId);

    IMongoDatabase database = serviceProvider.GetRequiredService<IMongoDatabase>();
    IMongoCollection<MongoLog> collection = database.GetCollection<MongoLog>("logs");
    List<MongoLog> logs = await collection.Find(Builders<MongoLog>.Filter.Eq(entity => entity.UniqueId, relationalLog.Id)).ToListAsync();
    MongoLog mongoLog = Assert.Single(logs);
    Assert.Equal(relationalLog.Id, mongoLog.UniqueId);
  }

  [Fact(DisplayName = "Nothing should happen when saving a log into no repository.")]
  public async Task Given_NoRepository_When_Save_Then_NothingHappens()
  {
    _loggingService.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");

    await _loggingService.CloseAndSaveAsync((int)HttpStatusCode.OK);
  }
}
