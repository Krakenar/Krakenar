using Krakenar.Core;
using Krakenar.Core.Logging;
using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Infrastructure;

public class EventBus : IEventBus
{
  protected virtual ILoggingService LoggingService { get; }
  protected virtual IServiceProvider ServiceProvider { get; }

  public EventBus(ILoggingService loggingService, IServiceProvider serviceProvider)
  {
    LoggingService = loggingService;
    ServiceProvider = serviceProvider;
  }

  public virtual async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    LoggingService.Report(@event);

    IEnumerable<object?> handlers = ServiceProvider.GetServices(typeof(IEventHandler<>).MakeGenericType(@event.GetType()));
    if (handlers.Any())
    {
      Type[] parameterTypes = [@event.GetType(), typeof(CancellationToken)];
      object[] parameters = [@event, cancellationToken];
      foreach (object? handler in handlers)
      {
        if (handler is not null)
        {
          MethodInfo? handle = handler.GetType().GetMethod("HandleAsync", parameterTypes);
          if (handle is not null)
          {
            await (Task)handle.Invoke(handler, parameters)!;
          }
        }
      }
    }
  }
}
