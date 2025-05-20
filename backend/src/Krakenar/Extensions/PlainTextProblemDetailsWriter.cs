using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Extensions;

internal class PlainTextProblemDetailsWriter : IProblemDetailsWriter
{
  private const string ContentType = MediaTypeNames.Text.Plain;

  public bool CanWrite(ProblemDetailsContext context) => context.HttpContext.Request.Headers.Accept.Contains(ContentType);

  public async ValueTask WriteAsync(ProblemDetailsContext context)
  {
    HttpContext httpContext = context.HttpContext;
    httpContext.Response.ContentType = ContentType;
    await httpContext.Response.WriteAsJsonAsync(context.ProblemDetails);
  }
}
