using Bogus;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Logging;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Commands;
using Krakenar.Core.Users.Events;
using Logitar;
using Logitar.EventSourcing;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;
using CoreLog = Krakenar.Core.Logging.Log;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using SessionDto = Krakenar.Contracts.Sessions.Session;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

[Trait(Traits.Category, Categories.Unit)]
public class LogTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should create a new relational log entity.")]
  public void Given_Log_When_ctor_Then_LogEntity()
  {
    CoreLog log = CoreLog.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    log.Operation = new Operation("User", "CreateAsync");
    log.Activity = new CreateOrReplaceUser(Id: null, new CreateOrReplaceUserPayload(_faker.Person.UserName), Version: null);

    log.Realm = new RealmDto
    {
      Id = Guid.NewGuid()
    };
    log.ApiKey = new ApiKeyDto
    {
      Id = Guid.NewGuid()
    };
    log.Session = new SessionDto
    {
      Id = Guid.NewGuid()
    };
    log.User = new UserDto
    {
      Id = Guid.NewGuid(),
      Realm = log.Realm
    };

    log.Report(new UserCreated(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), Password: null));
    log.Report(new Exception("Invalid request."));
    log.Close((int)HttpStatusCode.OK);
    Assert.True(log.EndedOn.HasValue);

    Log entity = new(log);

    Assert.Equal(log.Id, entity.Id);
    Assert.Equal(log.CorrelationId, entity.CorrelationId);
    Assert.Equal(log.Method, entity.Method);
    Assert.Equal(log.Destination, entity.Destination);
    Assert.Equal(log.Source, entity.Source);
    Assert.Equal(log.AdditionalInformation, entity.AdditionalInformation);
    Assert.Equal(log.Operation.Type, entity.OperationType);
    Assert.Equal(log.Operation.Name, entity.OperationName);
    Assert.Equal(log.Activity.GetType().GetNamespaceQualifiedName(), entity.ActivityType);
    Assert.Equal(JsonSerializer.Serialize(log.Activity, log.Activity.GetType()), entity.ActivityData);
    Assert.Equal(log.StatusCode, entity.StatusCode);
    Assert.Equal(log.IsCompleted, entity.IsCompleted);
    Assert.Equal(log.Level, entity.Level);
    Assert.Equal(log.HasErrors, entity.HasErrors);
    Assert.Equal(log.StartedOn.AsUniversalTime(), entity.StartedOn);
    Assert.Equal(log.EndedOn.Value.AsUniversalTime(), entity.EndedOn);
    Assert.Equal(log.Duration, entity.Duration);
    Assert.Equal(log.RealmId?.Value, entity.RealmId);
    Assert.Equal(log.ApiKeyId?.Value, entity.ApiKeyId);
    Assert.Equal(log.SessionId?.Value, entity.SessionId);
    Assert.Equal(log.UserId?.Value, entity.UserId);
    Assert.Equal(log.ActorId?.Value, entity.ActorId);

    Assert.Equal(log.Events.Count, entity.Events.Count);
    foreach (IEvent @event in log.Events)
    {
      Assert.Contains(
        entity.Events,
        e => e.Log == entity && e.LogId == entity.LogId && e.LogUid == entity.Id && e.EventId == ((IIdentifiableEvent)@event).Id.Value);
    }

    Assert.Equal(log.Exceptions.Count, entity.Exceptions.Count);
    foreach (Exception exception in log.Exceptions)
    {
      Assert.Contains(entity.Exceptions, e => e.Id != default
        && e.Log == entity && e.LogId == entity.LogId && e.LogUid == entity.Id
        && e.Type == exception.GetType().GetNamespaceQualifiedName() && e.Message == exception.Message
        && e.HResult == exception.HResult && e.HelpLink == exception.HelpLink && e.Source == exception.Source
        && e.StackTrace == exception.StackTrace && e.TargetSite == exception.TargetSite?.ToString());
    }
  }
}
