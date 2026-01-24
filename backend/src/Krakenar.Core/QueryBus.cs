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
    exception = exception.Unwrap();

    bool shouldRetry = exception is not TooManyResultsException;
    if (shouldRetry)
    {
      LoggingService?.Report(exception);
    }

    return shouldRetry;
  }
}
