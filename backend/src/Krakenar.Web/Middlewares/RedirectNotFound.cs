using Krakenar.Web.Settings;
using Microsoft.AspNetCore.Http.Extensions;

namespace Krakenar.Web.Middlewares;

public class RedirectNotFound
{
  protected virtual RequestDelegate Next { get; }

  public RedirectNotFound(RequestDelegate next)
  {
    Next = next;
  }

  public virtual async Task InvokeAsync(HttpContext context, AdminSettings adminSettings)
  {
    await Next(context);

    if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Request.Path.StartsWithSegments("/api"))
    {
      context.Response.Redirect($"/{adminSettings.BasePath}/{UriHelper.GetEncodedPathAndQuery(context.Request).Trim('/')}");
    }
  }
}
