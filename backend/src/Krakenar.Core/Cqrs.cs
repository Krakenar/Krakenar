using Logitar.EventSourcing;

namespace Krakenar.Core;

public interface ICommand;
public interface ICommand<TResult>;

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

public interface IQuery<TResult>;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
  Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
