using Krakenar.Core.Logging;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Infrastructure;

public class EventBus : Logitar.EventSourcing.Infrastructure.EventBus
{
  protected virtual ILoggingService? LoggingService { get; }

  public EventBus(IServiceProvider serviceProvider) : base(serviceProvider)
  {
    LoggingService = serviceProvider.GetService<ILoggingService>();
  }

  public override Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    LoggingService?.Report(@event);
    return base.PublishAsync(@event, cancellationToken);
  }
}
