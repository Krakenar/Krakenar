using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Logging;
using Logitar.EventSourcing;

namespace Krakenar.Logging;

internal class FakeLoggingService : ILoggingService // TODO(fpion): remove this
{
  public void Open(string traceIdentifier, string method, string url, string? ipAddress, string additionalInformation)
  {
  }

  public void Report(IEvent @event)
  {
  }

  public void Report(Exception exception)
  {
  }

  public void SetActivity<TResult>(ICommand<TResult> command)
  {

  }

  public void SetActivity<TResult>(IQuery<TResult> command)
  {

  }

  public void SetApiKey(ApiKey? apiKey)
  {
  }

  public void SetOperation(Operation operation)
  {
  }

  public void SetRealm(Realm? realm)
  {
  }

  public void SetSession(Session? session)
  {
  }

  public void SetUser(User? user)
  {
  }

  public Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
