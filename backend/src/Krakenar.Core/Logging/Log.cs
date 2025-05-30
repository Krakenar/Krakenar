using Krakenar.Contracts.Actors;
using Krakenar.Core.Actors;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Microsoft.Extensions.Logging;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using SessionDto = Krakenar.Contracts.Sessions.Session;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Logging;

public class Log
{
  public Guid Id { get; private set; }

  public string? CorrelationId { get; private set; }
  public string? Method { get; private set; }
  public string? Destination { get; private set; }
  public string? Source { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public Operation? Operation { get; set; }
  public IActivity? Activity { get; set; }

  public int? StatusCode { get; private set; }
  public bool IsCompleted => StatusCode.HasValue;
  public void Close(int statusCode, DateTime? endedOn = null)
  {
    StatusCode = statusCode;

    EndedOn = endedOn ?? DateTime.Now;
  }

  public LogLevel Level => HasErrors ? LogLevel.Error : LogLevel.Information;
  public bool HasErrors => Exceptions.Count > 0;

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;

  public RealmDto? Realm { get; set; }
  public RealmId? RealmId => Realm is null ? null : new(Realm.Id);
  public ApiKeyDto? ApiKey { get; set; }
  public ApiKeyId? ApiKeyId
  {
    get
    {
      if (ApiKey is null)
      {
        return null;
      }
      RealmId? realmId = ApiKey.Realm is null ? null : new(ApiKey.Realm.Id);
      return new ApiKeyId(ApiKey.Id, realmId);
    }
  }
  public SessionDto? Session { get; set; }
  public SessionId? SessionId
  {
    get
    {
      if (Session is null)
      {
        return null;
      }
      RealmId? realmId = Session.User.Realm is null ? null : new(Session.User.Realm.Id);
      return new SessionId(Session.Id, realmId);
    }
  }
  public UserDto? User { get; set; }
  public UserId? UserId
  {
    get
    {
      if (User is null)
      {
        return null;
      }
      RealmId? realmId = User.Realm is null ? null : new(User.Realm.Id);
      return new UserId(User.Id, realmId);
    }
  }
  public ActorId? ActorId
  {
    get
    {
      if (User is not null)
      {
        return new Actor(User).GetActorId();
      }
      else if (ApiKey is not null)
      {
        return new Actor(ApiKey).GetActorId();
      }
      return null;
    }
  }

  private readonly List<IEvent> _events = [];
  public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
  public void Report(IEvent @event)
  {
    _events.Add(@event);
  }

  private readonly List<Exception> _exceptions = [];
  public IReadOnlyCollection<Exception> Exceptions => _exceptions.AsReadOnly();
  public void Report(Exception exception)
  {
    _exceptions.Add(exception);
  }

  public static Log Open(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, Guid? id = null, DateTime? startedOn = null)
  {
    return new Log(correlationId, method, destination, source, additionalInformation, id, startedOn);
  }
  private Log(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, Guid? id = null, DateTime? startedOn = null)
  {
    Id = id ?? Guid.NewGuid();

    CorrelationId = correlationId;
    Method = method;
    Destination = destination;
    Source = source;
    AdditionalInformation = additionalInformation;

    StartedOn = startedOn ?? DateTime.Now;
  }

  public override bool Equals(object? obj) => obj is Log log && log.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
