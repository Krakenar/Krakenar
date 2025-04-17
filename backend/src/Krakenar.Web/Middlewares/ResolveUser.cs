using Krakenar.Contracts;
using Krakenar.Contracts.Constants;
using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Krakenar.Web.Middlewares;

public class ResolveUser
{
  protected virtual RequestDelegate Next { get; }
  protected virtual ProblemDetailsFactory ProblemDetailsFactory { get; }
  protected virtual IProblemDetailsService ProblemDetailsService { get; }

  public ResolveUser(RequestDelegate next, ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
  {
    Next = next;
    ProblemDetailsFactory = problemDetailsFactory;
    ProblemDetailsService = problemDetailsService;
  }

  public virtual async Task InvokeAsync(HttpContext context, IUserService userService)
  {
    if (context.User.Identity is not null
      && context.User.Identity.IsAuthenticated
      && context.Request.Headers.TryGetValue(Headers.User, out StringValues values))
    {
      IReadOnlyCollection<string> sanitized = values.Sanitize();
      if (sanitized.Count > 1)
      {
        Error error = new(code: "InvalidUserHeader", message: "Only one user header value is expected, but multiple were specified.");
        error.Data["Header"] = Headers.User;
        error.Data["SanitizedCount"] = sanitized.Count;
        error.Data["TotalCount"] = values.Count;
        await WriteResponseAsync(context, StatusCodes.Status400BadRequest, error);
        return;
      }
      else if (sanitized.Count == 1)
      {
        string value = sanitized.Single();
        bool parsed = Guid.TryParse(value, out Guid id);
        int index = value.IndexOf(':');
        CustomIdentifier? customIdentifier = index < 0 ? null : new(value[..index], value[(index + 1)..]);
        User? user = await userService.ReadAsync(parsed ? id : null, value, customIdentifier);
        if (user is null)
        {
          Error error = new(code: "UserNotFound", message: "The specified user could not be found.");
          error.Data["Realm"] = context.GetRealm()?.Id;
          error.Data["User"] = value;
          error.Data["Header"] = Headers.User;
          await WriteResponseAsync(context, StatusCodes.Status404NotFound, error);
          return;
        }

        context.SetUser(user);
      }

      await Next(context);
    }
  }

  protected virtual async Task WriteResponseAsync(HttpContext httpContext, int statusCode, Error error)
  {
    ProblemDetails problemDetails = ProblemDetailsFactory.CreateProblemDetails(httpContext, statusCode, error);

    httpContext.Response.StatusCode = statusCode;
    ProblemDetailsContext context = new()
    {
      HttpContext = httpContext,
      ProblemDetails = problemDetails
    };
    _ = await ProblemDetailsService.TryWriteAsync(context);
  }
}
