using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Krakenar.Web;

public class ExceptionHandler : IExceptionHandler
{
  protected virtual bool ExposeErrorDetail { get; }
  protected virtual ProblemDetailsFactory ProblemDetailsFactory { get; }
  protected virtual IProblemDetailsService ProblemDetailsService { get; }

  public ExceptionHandler(IConfiguration configuration, ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
  {
    string? exposeErrorDetailValue = Environment.GetEnvironmentVariable("EXPOSE_ERROR_DETAIL");
    ExposeErrorDetail = !string.IsNullOrWhiteSpace(exposeErrorDetailValue) && bool.TryParse(exposeErrorDetailValue, out bool exposeErrorDetail)
      ? exposeErrorDetail : configuration.GetValue<bool>("ExposeErrorDetail");

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
    else if (ExposeErrorDetail)
    {
      statusCode = StatusCodes.Status500InternalServerError;
    }

    if (statusCode is null)
    {
      return false;
    }

    Error error = ToError(exception);
    ProblemDetails problemDetails = ProblemDetailsFactory.CreateProblemDetails(
      httpContext,
      statusCode,
      title: FormatToTitle(error.Code),
      type: null,
      detail: error.Message,
      instance: httpContext.Request.GetDisplayUrl());
    problemDetails.Extensions["error"] = error;

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
    || exception is ValidationException;

  private static string FormatToTitle(string code)
  {
    List<string> words = new(capacity: code.Length);

    StringBuilder word = new();
    for (int i = 0; i < code.Length; i++)
    {
      char? previous = (i > 0) ? code[i - 1] : null;
      char current = code[i];
      char? next = (i < code.Length - 1) ? code[i + 1] : null;

      if (char.IsUpper(current) && ((previous.HasValue && char.IsLower(previous.Value)) || (next.HasValue && char.IsLower(next.Value))))
      {
        if (word.Length > 0)
        {
          words.Add(word.ToString());
          word.Clear();
        }
      }

      word.Append(current);
    }
    if (word.Length > 0)
    {
      words.Add(word.ToString());
    }

    return string.Join(' ', words);
  }

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
      error = new(exception.GetErrorCode(), exception.Message);
      error.Data[nameof(exception.HelpLink)] = exception.HelpLink;
      error.Data[nameof(exception.HResult)] = exception.HResult;
      error.Data[nameof(exception.InnerException)] = exception.InnerException is null ? null : ToError(exception.InnerException);
      error.Data[nameof(exception.Source)] = exception.Source;
      error.Data[nameof(exception.StackTrace)] = exception.StackTrace;
      error.Data[nameof(exception.TargetSite)] = exception.TargetSite?.ToString();

      Dictionary<string, object?> data = new(capacity: exception.Data.Count);
      foreach (DictionaryEntry entry in exception.Data)
      {
        try
        {
          string key = Serialize(entry.Key);
          _ = Serialize(entry.Value);
          data[key] = entry.Value;
        }
        catch (Exception)
        {
        }
      }
      error.Data[nameof(exception.Data)] = data;
    }
    return error;
  }

  private static readonly JsonSerializerOptions _serializerOptions = new();
  static ExceptionHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }
  private static string Serialize(object? value) => JsonSerializer.Serialize(value, _serializerOptions);
}
