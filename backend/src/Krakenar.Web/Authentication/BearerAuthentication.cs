using Krakenar.Contracts.Constants;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Krakenar.Web.Authentication;

public class BearerAuthenticationOptions : AuthenticationSchemeOptions;

public class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  protected virtual IOpenAuthenticationService OpenAuthenticationService { get; }

  public BearerAuthenticationHandler(IOpenAuthenticationService openAuthenticationService, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    OpenAuthenticationService = openAuthenticationService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      IReadOnlyCollection<string> sanitized = authorization.Sanitize();
      if (sanitized.Count > 1)
      {
        Logger.LogWarning("Multiple {Header} header values were received ({Sanitized} sanitized, {Total} total). Ignoring {Scheme} authentication.",
          Headers.Authorization, sanitized.Count, authorization.Count, Scheme.Name);
      }
      else if (sanitized.Count == 1)
      {
        string value = sanitized.Single();
        string[] values = value.Split();
        if (values.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (values[0] == Schemes.Bearer)
        {
          try
          {
            User user = await OpenAuthenticationService.GetUserAsync(accessToken: values[1]);
            Session? session = user.Sessions.Count == 1 ? user.Sessions.Single() : null;

            ClaimsPrincipal principal;
            Context.SetUser(user);
            if (session is null)
            {
              principal = new(user.CreateClaimsIdentity(Scheme.Name));
            }
            else
            {
              Context.SetSession(session);
              principal = new(session.CreateClaimsIdentity(Scheme.Name));
            }
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
