using Bogus;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Events;
using Microsoft.Extensions.Logging;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using SessionDto = Krakenar.Contracts.Sessions.Session;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Logging;

[Trait(Traits.Category, Categories.Unit)]
public class LogTests
{
  private readonly Faker _faker = new();
  private readonly Log _log;

  public LogTests()
  {
    _log = Log.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent}""}}");
  }

  [Theory(DisplayName = "ActorId: it should return the correct actor ID from an API key.")]
  [InlineData(null)]
  [InlineData("736c32b3-7630-42dc-addd-07c47c6318c7")]
  public void Given_ApiKey_When_ActorId_Then_CorrectId(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    ApiKeyId apiKeyId = new(Guid.NewGuid(), realmId);

    Assert.Null(_log.ActorId);

    _log.ApiKey = new ApiKeyDto
    {
      Id = apiKeyId.EntityId
    };
    if (realmId.HasValue)
    {
      _log.ApiKey.Realm = new RealmDto
      {
        Id = realmId.Value.ToGuid()
      };
    }

    Assert.True(_log.ActorId.HasValue);
    Assert.Equal(apiKeyId.Value, _log.ActorId.Value.Value);
  }

  [Theory(DisplayName = "ActorId: it should return the correct actor ID from a user.")]
  [InlineData(null)]
  [InlineData("0cb7493a-e912-4ddb-89d5-08150fd7fe13")]
  public void Given_User_When_ActorId_Then_CorrectId(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    UserId userId = new(Guid.NewGuid(), realmId);

    Assert.Null(_log.ActorId);

    _log.User = new UserDto
    {
      Id = userId.EntityId
    };
    if (realmId.HasValue)
    {
      _log.User.Realm = new RealmDto
      {
        Id = realmId.Value.ToGuid()
      };
    }

    Assert.True(_log.ActorId.HasValue);
    Assert.Equal(userId.Value, _log.ActorId.Value.Value);
  }

  [Theory(DisplayName = "ApiKeyId: it should return the correct API key ID.")]
  [InlineData(null)]
  [InlineData("ba20fc7a-3a6b-46d2-abc9-badebae1f981")]
  public void Given_ApiKey_When_ApiKeyId_Then_CorrectId(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    ApiKeyId apiKeyId = new(Guid.NewGuid(), realmId);

    Assert.Null(_log.ApiKeyId);

    _log.ApiKey = new ApiKeyDto
    {
      Id = apiKeyId.EntityId
    };
    if (realmId.HasValue)
    {
      _log.ApiKey.Realm = new RealmDto
      {
        Id = realmId.Value.ToGuid()
      };
    }
    Assert.Equal(apiKeyId, _log.ApiKeyId);
  }

  [Theory(DisplayName = "Close: it should close an existing log.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_Arguments_When_Close_Then_LogClosed(bool minimal)
  {
    int delay = _faker.Random.Int(min: 0, max: 59);
    Log log = Log.Open(_log.CorrelationId, _log.Method, _log.Destination, _log.Source, _log.AdditionalInformation, _log.Id, DateTime.Now.AddSeconds(-delay));
    Assert.False(log.IsCompleted);

    int statusCode = (int)HttpStatusCode.OK;
    DateTime? endedOn = minimal ? null : DateTime.Now;

    log.Close(statusCode, endedOn);

    Assert.Equal(statusCode, log.StatusCode);
    Assert.True(log.IsCompleted);

    if (endedOn.HasValue)
    {
      Assert.Equal(endedOn.Value, log.EndedOn);
      Assert.Equal(endedOn.Value - log.StartedOn, log.Duration);
    }
    else
    {
      Assert.True(log.EndedOn.HasValue);
      Assert.Equal(log.EndedOn.Value - log.StartedOn, log.Duration);
    }
  }

  [Fact(DisplayName = "Equals: it should return false when the IDs are different.")]
  public void Given_DifferentIds_When_Equals_Then_False()
  {
    Log log = Log.Open(_log.CorrelationId, _log.Method, _log.Destination, _log.Source, _log.AdditionalInformation);
    Assert.NotEqual(_log.Id, log.Id);
    Assert.False(_log.Equals(log));
  }

  [Theory(DisplayName = "Equals: it should return false when the value is not a log.")]
  [InlineData(null)]
  [InlineData(123)]
  [InlineData("test")]
  public void Given_NotLog_When_Equals_Then_False(object? value)
  {
    Assert.False(_log.Equals(value));
  }

  [Fact(DisplayName = "Equals: it should return true when the IDs are equal.")]
  public void Given_EqualIds_When_Equals_Then_True()
  {
    Log log = Log.Open(_log.CorrelationId, _log.Method, _log.Destination, _log.Source, _log.AdditionalInformation, _log.Id, _log.StartedOn);
    Assert.True(_log.Equals(_log));
    Assert.True(_log.Equals(log));
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_Log_When_GetHashCode_Then_CorrectHashCode()
  {
    int hashCode = _log.Id.GetHashCode();
    Assert.Equal(hashCode, _log.GetHashCode());
  }

  [Theory(DisplayName = "Open: it should create a new log.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_Arguments_When_Open_Then_LogOpened(bool minimal)
  {
    string correlationId = Guid.NewGuid().ToString();
    string method = "GET";
    string destination = "/api/users/abc123";
    string source = _faker.Internet.Ip();
    string additionalInformation = $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}";
    Guid? id = minimal ? null : Guid.NewGuid();
    DateTime? startedOn = minimal ? null : DateTime.Now.AddDays(-10);

    Log log = Log.Open(correlationId, method, destination, source, additionalInformation, id, startedOn);

    Assert.Equal(correlationId, log.CorrelationId);
    Assert.Equal(method, log.Method);
    Assert.Equal(destination, log.Destination);
    Assert.Equal(source, log.Source);
    Assert.Equal(additionalInformation, log.AdditionalInformation);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, log.Id);
    }
    else
    {
      Assert.NotEqual(default, log.Id);
    }

    if (startedOn.HasValue)
    {
      Assert.Equal(startedOn.Value, log.StartedOn);
    }
    else
    {
      Assert.NotEqual(default, log.StartedOn);
    }
  }

  [Fact(DisplayName = "RealmId: it should return the correct ID.")]
  public void Given_Realm_When_RealmId_Then_CorrectId()
  {
    Assert.Null(_log.RealmId);

    _log.Realm = new RealmDto
    {
      Id = Guid.NewGuid()
    };
    Assert.Equal(_log.RealmId, new RealmId(_log.Realm.Id));
  }

  [Fact(DisplayName = "Report: it should report an event.")]
  public void Given_Event_When_Report_Then_EventReported()
  {
    Assert.Empty(_log.Events);

    UserCreated @event = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), Password: null);
    _log.Report(@event);

    Assert.Same(@event, Assert.Single(_log.Events));
  }

  [Fact(DisplayName = "Report: it should report an exception.")]
  public void Given_Exception_When_Report_Then_ExceptionReported()
  {
    Assert.False(_log.HasErrors);
    Assert.Equal(LogLevel.Information, _log.Level);
    Assert.Empty(_log.Exceptions);

    Exception exception = new("Invalid.");
    _log.Report(exception);

    Assert.True(_log.HasErrors);
    Assert.Equal(LogLevel.Error, _log.Level);
    Assert.Same(exception, Assert.Single(_log.Exceptions));
  }

  [Theory(DisplayName = "SessionId: it should return the correct session ID.")]
  [InlineData(null)]
  [InlineData("b3c62d84-60c0-4471-8486-b678dda48edc")]
  public void Given_Session_When_SessionId_Then_CorrectId(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    SessionId sessionId = new(Guid.NewGuid(), realmId);

    Assert.Null(_log.Session);

    _log.Session = new SessionDto
    {
      Id = sessionId.EntityId
    };
    if (realmId.HasValue)
    {
      _log.Session.User.Realm = new RealmDto
      {
        Id = realmId.Value.ToGuid()
      };
    }
    Assert.Equal(sessionId, _log.SessionId);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Log_When_GetString_Then_CorrectString()
  {
    string expected = $"{typeof(Log)} (Id={_log.Id})";
    Assert.Equal(expected, _log.ToString());
  }

  [Theory(DisplayName = "UserId: it should return the correct user ID.")]
  [InlineData(null)]
  [InlineData("fa98fec1-69e1-4cbf-a8f0-2b2352956b40")]
  public void Given_User_When_UserId_Then_CorrectId(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    UserId userId = new(Guid.NewGuid(), realmId);

    Assert.Null(_log.UserId);

    _log.User = new UserDto
    {
      Id = userId.EntityId
    };
    if (realmId.HasValue)
    {
      _log.User.Realm = new RealmDto
      {
        Id = realmId.Value.ToGuid()
      };
    }
    Assert.Equal(userId, _log.UserId);
  }
}
