using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Core;
using Krakenar.Web.Settings;
using Logitar;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Krakenar.Web;

public class ExceptionHandler : IExceptionHandler
{
  protected virtual ErrorSettings ErrorSettings { get; }
  protected virtual ProblemDetailsFactory ProblemDetailsFactory { get; }
  protected virtual IProblemDetailsService ProblemDetailsService { get; }

  public ExceptionHandler(ErrorSettings errorSettings, ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
  {
    ErrorSettings = errorSettings;
    ProblemDetailsFactory = problemDetailsFactory;
    ProblemDetailsService = problemDetailsService;
  }

  public virtual async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    int? statusCode = null;
    if (IsBadRequest(exception))
    {
      statusCode = StatusCodes.Status400BadRequest;
    }
    else if (exception is NotFoundException)
    {
      statusCode = StatusCodes.Status404NotFound;
    }
    else if (exception is ConflictException)
    {
      statusCode = StatusCodes.Status409Conflict;
    }
    else if (ErrorSettings.ExposeDetail)
    {
      statusCode = StatusCodes.Status500InternalServerError;
    }

    if (statusCode is null)
    {
      return false;
    }

    Error error = ToError(exception);
    ProblemDetails problemDetails = ProblemDetailsFactory.CreateProblemDetails(httpContext, statusCode.Value, error);

    httpContext.Response.StatusCode = statusCode.Value;
    ProblemDetailsContext context = new()
    {
      HttpContext = httpContext,
      ProblemDetails = problemDetails,
      Exception = exception
    };
    return await ProblemDetailsService.TryWriteAsync(context);
  }

  private static bool IsBadRequest(Exception exception) => exception is BadRequestException
    || exception is InvalidCredentialsException
    || exception is SecurityTokenArgumentException
    || exception is SecurityTokenException
    || exception is TooManyResultsException
    || exception is ValidationException;

  private static Error ToError(Exception exception)
  {
    Error error;
    if (exception is ErrorException errorException)
    {
      error = errorException.Error;
    }
    else if (exception is ValidationException validation)
    {
      error = new(exception.GetErrorCode(), "Validation failed.");
      error.Data["Failures"] = validation.Errors;
    }
    else
    {
      error = new(exception);
    }
    return error;
  }
}
