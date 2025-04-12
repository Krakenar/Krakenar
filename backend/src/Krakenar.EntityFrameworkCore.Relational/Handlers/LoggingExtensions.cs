using Krakenar.Contracts;
using Logitar;
using Logitar.EventSourcing;
using Microsoft.Extensions.Logging;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public static class LoggingExtensions
{
  public static void LogSuccess<T>(this ILogger<T> logger, DomainEvent @event)
  {
    logger.LogInformation("{Timestamp}|{EventType}:{EventId}|success|The event was handled successfully.", DateTime.Now.ToISOString(), @event.GetType().Name, @event.Id);
  }

  public static void LogUnexpectedVersion<T>(this ILogger<T> logger, DomainEvent @event, AggregateEntity? entity = null)
  {
    Error error = new(code: "UnexpectedVersion", message: "The entity version did not match the expected version.");
    error.Data["ExpectedVersion"] = @event.Version - 1;
    error.Data["ActualVersion"] = entity?.Version;
    string json = JsonSerializer.Serialize(error);

    logger.LogWarning("{Timestamp}|{EventType}:{EventId}|error|{Error}", DateTime.Now.ToISOString(), @event.GetType().Name, @event.Id, json);
  }
}
