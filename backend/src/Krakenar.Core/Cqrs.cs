using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Core.Logging;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Krakenar.Core;

public interface IActivity;
public interface ISensitiveActivity
{
  IActivity Anonymize();
}

public interface ICommand : IActivity;
public interface ICommand<TResult> : IActivity;

public interface ICommandBus
{
  Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
  Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
  Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface IEventHandler<TEvent> where TEvent : IEvent
{
  Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

public interface IQuery<TResult> : IActivity;

public interface IQueryBus
{
  Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> command, CancellationToken cancellationToken = default);
}

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
  Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

public class CommandBus : ICommandBus
{
  protected virtual ILoggingService LoggingService { get; }
  protected virtual Random Random { get; } = new();
  protected virtual RetrySettings RetrySettings { get; }
  protected virtual IServiceProvider ServiceProvider { get; }

  public CommandBus(ILoggingService loggingService, RetrySettings retrySettings, IServiceProvider serviceProvider)
  {
    LoggingService = loggingService;
    RetrySettings = retrySettings;
    ServiceProvider = serviceProvider;
  }

  public virtual async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
  {
    LoggingService.SetActivity(command);

    Type commandType = command.GetType();
    IEnumerable<object?> handlers = ServiceProvider.GetServices(typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult)));
    int count = handlers.Count();
    if (count != 1)
    {
      throw new InvalidOperationException($"Exactly 1 command handler was expected, but {(count < 1 ? "none was" : $"{count} were")} found for command type '{command.GetType()}'.");
    }
    object handler = handlers.Single() ?? throw new InvalidOperationException($"The command handler cannot be null for command type '{commandType}'.");

    Type[] parameterTypes = [commandType, typeof(CancellationToken)];
    MethodInfo handle = handler.GetType().GetMethod("HandleAsync", parameterTypes)
      ?? throw new NotImplementedException($"The HandleAsync method was not found on handler for command type '{commandType}'.");

    return await ExecuteWithRetryAsync(command, handle, handler, cancellationToken);
  }
  protected virtual async Task<TResult> ExecuteWithRetryAsync<TResult>(ICommand<TResult> command, MethodInfo handle, object handler, CancellationToken cancellationToken)
  {
    RetryAlgorithm algorithm = RetrySettings.Algorithm;
    int delay = RetrySettings.Delay;
    if (delay < 1)
    {
      algorithm = RetryAlgorithm.None;
    }
    int exponentialBase = RetrySettings.ExponentialBase;
    if (algorithm == RetryAlgorithm.Exponential && exponentialBase <= 1)
    {
      algorithm = RetryAlgorithm.None;
    }
    int randomVariation = RetrySettings.RandomVariation;
    if (algorithm == RetryAlgorithm.Random && (randomVariation < 1 || randomVariation >= delay))
    {
      algorithm = RetryAlgorithm.None;
    }
    int maximumRetries = RetrySettings.MaximumRetries;
    int maximumDelay = RetrySettings.MaximumDelay;

    ILogger<CommandBus>? logger = null;

    object[] parameters = [command, cancellationToken];
    Exception? innerException;
    int attempt = 0;
    while (true)
    {
      try
      {
        return await (Task<TResult>)handle.Invoke(handler, parameters)!;
      }
      catch (Exception exception)
      {
        if (algorithm == RetryAlgorithm.None || IgnoreException(exception))
        {
          throw;
        }
        innerException = exception;

        if (maximumRetries > 0 && attempt > maximumRetries)
        {
          break;
        }
        attempt++;

        int millisecondsDelay = CalculateDelay(algorithm, attempt, delay, exponentialBase, randomVariation);
        if (maximumDelay > 0 && millisecondsDelay > maximumDelay)
        {
          break;
        }

        LoggingService.Report(exception);

        logger ??= ServiceProvider.GetService<ILogger<CommandBus>>();
        logger?.LogWarning(exception, "Command '{Command}' execution failed at attempt {Attempt}, will retry in {Delay}ms.", command.GetType(), attempt, millisecondsDelay);

        await Task.Delay(millisecondsDelay, cancellationToken);
      }
    }

    throw new InvalidOperationException($"Command '{command.GetType()}' execution failed after {attempt} attempts. See inner exception for more detail.", innerException);
  }
  protected virtual int CalculateDelay(RetryAlgorithm algorithm, int attempt, int delay, int exponentialBase, int randomVariation)
  {
    switch (algorithm)
    {
      case RetryAlgorithm.Exponential:
        return (int)(Math.Pow(exponentialBase, attempt - 1) * delay);
      case RetryAlgorithm.Fixed:
        return delay;
      case RetryAlgorithm.Linear:
        return attempt * delay;
      case RetryAlgorithm.Random:
        int minimum = delay - randomVariation;
        int maximum = delay + randomVariation;
        return Random.Next(minimum, maximum + 1);
      default:
        throw new ArgumentException($"The retry algorithm '{algorithm}' is not supported.", nameof(algorithm));
    }
  }
  protected virtual bool IgnoreException(Exception exception) => exception is BadRequestException
    || exception is ConflictException
    || exception is InvalidCredentialsException
    || exception is NotFoundException
    || exception is SecurityTokenArgumentException
    || exception is SecurityTokenException
    || exception is TooManyResultsException
    || exception is ValidationException;
}

public class QueryBus : IQueryBus
{
  protected virtual ILoggingService LoggingService { get; }
  protected virtual IServiceProvider ServiceProvider { get; }

  public QueryBus(ILoggingService loggingService, IServiceProvider serviceProvider)
  {
    LoggingService = loggingService;
    ServiceProvider = serviceProvider;
  }

  public virtual async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
  {
    LoggingService.SetActivity(query);

    Type queryType = query.GetType();
    IEnumerable<object?> handlers = ServiceProvider.GetServices(typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult)));
    int count = handlers.Count();
    if (count != 1)
    {
      throw new InvalidOperationException($"Exactly 1 query handler was expected, but {(count < 1 ? "none was" : $"{count} were")} found for query type '{query.GetType()}'.");
    }
    object handler = handlers.Single() ?? throw new InvalidOperationException($"The query handler cannot be null for query type '{queryType}'.");

    Type[] parameterTypes = [queryType, typeof(CancellationToken)];
    MethodInfo handle = handler.GetType().GetMethod("HandleAsync", parameterTypes)
      ?? throw new NotImplementedException($"The HandleAsync method was not found on handler for query type '{queryType}'.");

    object[] parameters = [query, cancellationToken];
    return await (Task<TResult>)handle.Invoke(handler, parameters)!;
  }
}
