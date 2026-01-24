using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Core.Logging;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Krakenar.Core;

public class CommandBus : Logitar.CQRS.CommandBus
{
  protected virtual ILoggingService? LoggingService { get; }

  public CommandBus(IServiceProvider serviceProvider) : base(serviceProvider)
  {
    LoggingService = serviceProvider.GetService<ILoggingService>();
  }

  public override Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
  {
    LoggingService?.SetActivity(command);
    return base.ExecuteAsync(command, cancellationToken);
  }

  protected override bool ShouldRetry<TResult>(ICommand<TResult> command, Exception exception)
  {
    exception = exception.Unwrap();

    bool shouldRetry = exception is not BadRequestException
      && exception is not ConflictException
      && exception is not InvalidCredentialsException
      && exception is not NotFoundException
      && exception is not SecurityTokenArgumentException
      && exception is not SecurityTokenException
      && exception is not TooManyResultsException
      && exception is not ValidationException;
    if (shouldRetry)
    {
      LoggingService?.Report(exception);
    }

    return shouldRetry;
  }
}
