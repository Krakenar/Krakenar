using Krakenar.Contracts.Constants;
using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Krakenar.Web.Authentication;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  protected virtual IUserService UserService { get; }

  public BasicAuthenticationHandler(IUserService userService, IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    UserService = userService;
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
        else if (values[0] == Schemes.Basic)
        {
          byte[] bytes = Convert.FromBase64String(values[1]);
          string credentials = Encoding.UTF8.GetString(bytes);
          int index = credentials.IndexOf(':');
          if (index <= 0)
          {
            return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
          }

          try
          {
            AuthenticateUserPayload payload = new(user: credentials[..index], password: credentials[(index + 1)..]);
            User user = await UserService.AuthenticateAsync(payload);

            Context.SetUser(user);

            ClaimsPrincipal principal = new(user.CreateClaimsIdentity(Scheme.Name));
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
