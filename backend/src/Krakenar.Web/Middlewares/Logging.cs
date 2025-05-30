using Krakenar.Core.Logging;
using Microsoft.AspNetCore.Http.Extensions;

namespace Krakenar.Web.Middlewares;

public class Logging
{
  protected virtual RequestDelegate Next { get; }

  public Logging(RequestDelegate next)
  {
    Next = next;
  }

  public virtual async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
  {
    HttpRequest request = context.Request;
    loggingService.Open(context.TraceIdentifier, request.Method, request.GetDisplayUrl(), context.GetClientIpAddress(), context.GetAdditionalInformation());

    try
    {
      await Next(context);
    }
    catch (Exception exception)
    {
      loggingService.Report(exception);

      throw;
    }
    finally
    {
      HttpResponse response = context.Response;
      await loggingService.CloseAndSaveAsync(response.StatusCode);
    }
  }
}
