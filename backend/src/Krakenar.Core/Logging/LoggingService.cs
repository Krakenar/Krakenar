using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Logging;

public class LoggingService : ILoggingService
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual Log? Log { get; set; }

  public LoggingService(IApplicationContext applicationContext)
  {
    ApplicationContext = applicationContext;
  }

  public virtual void Open(string correlationId, string method, string url, string? ipAddress, string additionalInformation)
  {
    if (Log is not null)
    {
      throw new InvalidOperationException($"You must close the current log by calling one of the '{nameof(CloseAndSaveAsync)}' methods before opening a new log.");
    }
    Log = Log.Open(correlationId, method, url, ipAddress, additionalInformation);
  }

  public virtual void Report(IEvent @event)
  {
    Log?.Report(@event);
  }
  public virtual void Report(Exception exception)
  {
    Log?.Report(exception);
  }

  public virtual void SetActivity(IActivity activity)
  {
    if (Log is not null)
    {
      Log.Activity = activity; // TODO(fpion): anonymize
    }
  }

  public virtual void SetOperation(Operation operation)
  {
    if (Log is not null)
    {
      Log.Operation = operation;
    }
  }

  public virtual void SetRealm(Realm? realm)
  {
    if (Log is not null)
    {
      Log.Realm = realm;
    }
  }
  public virtual void SetApiKey(ApiKey? apiKey)
  {
    if (Log is not null)
    {
      Log.ApiKey = apiKey;
    }
  }
  public virtual void SetSession(Session? session)
  {
    if (Log is not null)
    {
      Log.Session = session;
    }
  }
  public virtual void SetUser(User? user)
  {
    if (Log is not null)
    {
      Log.User = user;
    }
  }

  public virtual async Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken)
  {
    if (Log is null)
    {
      throw new InvalidOperationException($"You must open a new log by calling one of the '{nameof(Open)}' methods before calling the current method.");
    }
    Log.Close(statusCode);

    if (ShouldSaveLog())
    {
      await Task.Delay(1, cancellationToken); //await _logRepository.SaveAsync(Log, cancellationToken); // TODO(fpion): implement
    }

    Log = null;
  }

  protected virtual bool ShouldSaveLog()
  {
    ILoggingSettings loggingSettings = ApplicationContext.LoggingSettings;
    if (Log is not null)
    {
      if (!loggingSettings.OnlyErrors || Log.HasErrors)
      {
        switch (loggingSettings.Extent)
        {
          case LoggingExtent.ActivityOnly:
            return Log.Activity is not null;
          case LoggingExtent.Full:
            return true;
        }
      }
    }

    return false;
  }
}
