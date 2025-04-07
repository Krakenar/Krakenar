namespace Krakenar.Core;

public interface ICommand;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
  Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQuery<TResult>;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
  Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
