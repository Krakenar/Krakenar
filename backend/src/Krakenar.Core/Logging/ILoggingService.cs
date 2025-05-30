using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Logging;

public interface ILoggingService
{
  void Open(string traceIdentifier, string method, string url, string? ipAddress, string additionalInformation);
  void Report(IEvent @event);
  void Report(Exception exception);
  // TODO(fpion): SetActivity
  void SetOperation(Operation operation);
  void SetRealm(Realm? realm);
  void SetApiKey(ApiKey? apiKey);
  void SetSession(Session? session);
  void SetUser(User? user);
  Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken = default);
}
