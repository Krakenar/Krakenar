using Krakenar.Core;
using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Infrastructure;

public class EventBus : IEventBus
{
  protected virtual IServiceProvider ServiceProvider { get; }

  public EventBus(IServiceProvider serviceProvider)
  {
    ServiceProvider = serviceProvider;
  }

  public virtual async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
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
