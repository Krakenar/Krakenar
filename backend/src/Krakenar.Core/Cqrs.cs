using Krakenar.Core.Logging;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Core;

public interface IActivity;

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
  protected virtual IServiceProvider ServiceProvider { get; }

  public CommandBus(ILoggingService loggingService, IServiceProvider serviceProvider)
  {
    LoggingService = loggingService;
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

    object[] parameters = [command, cancellationToken];
    return await (Task<TResult>)handle.Invoke(handler, parameters)!;
  }
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
