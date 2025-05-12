using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Authorization;

namespace Krakenar.Web.Authorization;

public record KrakenarAdminRequirement : IAuthorizationRequirement;

public class KrakenarAdminHandler : AuthorizationHandler<KrakenarAdminRequirement>
{
  protected virtual IHttpContextAccessor HttpContextAccessor { get; }

  public KrakenarAdminHandler(IHttpContextAccessor httpContextAccessor)
  {
    HttpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, KrakenarAdminRequirement requirement)
  {
    HttpContext? httpContext = HttpContextAccessor.HttpContext;
    if (httpContext is not null)
    {
      ApiKey? apiKey = httpContext.GetApiKey();
      if (apiKey is not null)
      {
        if (apiKey.Realm is null)
        {
          context.Succeed(requirement);
        }
        else
        {
          context.Fail(new AuthorizationFailureReason(this, "The API key should not belong to a realm."));
        }
      }

      User? user = httpContext.GetUser();
      if (user is not null)
      {
        if (user.Realm is null)
        {
          context.Succeed(requirement);
        }
        else
        {
          context.Fail(new AuthorizationFailureReason(this, "The user should not belong to a realm."));
        }
      }
    }

    return Task.CompletedTask;
  }
}
