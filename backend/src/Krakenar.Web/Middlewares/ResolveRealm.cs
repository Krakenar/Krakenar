using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Web.Constants;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Krakenar.Web.Middlewares;

public class ResolveRealm
{
  protected virtual RequestDelegate Next { get; }
  protected virtual ProblemDetailsFactory ProblemDetailsFactory { get; }
  protected virtual IProblemDetailsService ProblemDetailsService { get; }

  public ResolveRealm(RequestDelegate next, ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
  {
    Next = next;
    ProblemDetailsFactory = problemDetailsFactory;
    ProblemDetailsService = problemDetailsService;
  }

  public virtual async Task InvokeAsync(HttpContext context, IRealmService realmService)
  {
    if (context.User.Identity is not null
      && context.User.Identity.IsAuthenticated
      && context.Request.Headers.TryGetValue(Headers.Realm, out StringValues values))
    {
      IReadOnlyCollection<string> sanitized = values.Sanitize();
      if (sanitized.Count > 1)
      {
        Error error = new(code: "InvalidRealmHeader", message: "Only one realm header value is expected, but multiple were specified.");
        error.Data["Header"] = Headers.Realm;
        error.Data["SanitizedCount"] = sanitized.Count;
        error.Data["TotalCount"] = values.Count;
        await WriteResponseAsync(StatusCodes.Status400BadRequest, error, context);
        return;
      }
      else if (sanitized.Count == 1)
      {
        string value = sanitized.Single();
        bool parsed = Guid.TryParse(value, out Guid id);
        Realm? realm = await realmService.ReadAsync(parsed ? id : null, value);
        if (realm is null)
        {
          Error error = new(code: "RealmNotFound", message: "The specified realm could not be found.");
          error.Data["Realm"] = value;
          error.Data["Header"] = Headers.Realm;
          await WriteResponseAsync(StatusCodes.Status404NotFound, error, context);
          return;
        }

        context.SetRealm(realm);
      }

      await Next(context);
    }
  }

  protected virtual async Task WriteResponseAsync(int statusCode, Error error, HttpContext httpContext)
  {
    ProblemDetails problemDetails = ProblemDetailsFactory.CreateProblemDetails(
      httpContext,
      statusCode,
      title: error.Code.FormatToTitle(),
      type: null,
      detail: error.Message,
      instance: httpContext.Request.GetDisplayUrl());
    problemDetails.Extensions["error"] = error;

    httpContext.Response.StatusCode = statusCode;
    ProblemDetailsContext context = new()
    {
      HttpContext = httpContext,
      ProblemDetails = problemDetails
    };
    _ = await ProblemDetailsService.TryWriteAsync(context);
  }
}
