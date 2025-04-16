using Krakenar.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Krakenar.Web;

public static class ProblemDetailsExtensions
{
  public static ProblemDetails CreateProblemDetails(this ProblemDetailsFactory factory, HttpContext httpContext, int statusCode, Error error)
  {
    ProblemDetails problemDetails = factory.CreateProblemDetails(
      httpContext,
      statusCode,
      title: error.Code.FormatToTitle(),
      type: null,
      detail: error.Message,
      instance: httpContext.Request.GetDisplayUrl());

    problemDetails.Extensions["error"] = error;

    return problemDetails;
  }
}
