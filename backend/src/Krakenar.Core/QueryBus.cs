using Krakenar.Contracts;
using Krakenar.Core.Logging;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Core;

public class QueryBus : Logitar.CQRS.QueryBus
{
  protected virtual ILoggingService? LoggingService { get; }

  public QueryBus(IServiceProvider serviceProvider) : base(serviceProvider)
  {
    LoggingService = serviceProvider.GetService<ILoggingService>();
  }

  public override Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
  {
    LoggingService?.SetActivity(query);
    return base.ExecuteAsync(query, cancellationToken);
  }

  protected override bool ShouldRetry<TResult>(IQuery<TResult> command, Exception exception)
  {
    return exception is not TooManyResultsException;
  }

  // TODO(fpion): Exceptions are not reported to the LoggingService!
}
