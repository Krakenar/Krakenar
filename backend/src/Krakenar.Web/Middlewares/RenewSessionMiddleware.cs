using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Web.Constants;

namespace Krakenar.Web.Middlewares;

public class RenewSessionMiddleware
{
  protected virtual RequestDelegate Next { get; }

  public RenewSessionMiddleware(RequestDelegate next)
  {
    Next = next;
  }

  public virtual async Task InvokeAsync(HttpContext context, ICommandHandler<RenewSession, Session> renewSession)
  {
    if (!context.IsSignedIn())
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken is not null)
      {
        try
        {
          RenewSessionPayload payload = new(refreshToken, context.GetSessionCustomAttributes());
          RenewSession command = new(payload);
          Session session = await renewSession.HandleAsync(command);
          context.SignIn(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(Cookies.RefreshToken);
        }
      }
    }

    await Next(context);
  }
}
