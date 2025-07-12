using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Krakenar.Web.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
  protected virtual IApiKeyService ApiKeyService { get; }

  public ApiKeyAuthenticationHandler(IApiKeyService apiKeyService, IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    ApiKeyService = apiKeyService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.ApiKey, out StringValues xApiKey))
    {
      IReadOnlyCollection<string> sanitized = xApiKey.Sanitize();
      if (sanitized.Count > 1)
      {
        Logger.LogWarning("Multiple {Header} header values were received ({Sanitized} sanitized, {Total} total). Ignoring {Scheme} authentication.",
          Headers.ApiKey, sanitized.Count, xApiKey.Count, Scheme.Name);
      }
      else if (sanitized.Count == 1)
      {
        try
        {
          AuthenticateApiKeyPayload payload = new(xApiKey: sanitized.Single());
          ApiKey apiKey = await ApiKeyService.AuthenticateAsync(payload);

          Context.SetApiKey(apiKey);

          ClaimsPrincipal principal = new(apiKey.CreateClaimsIdentity(Scheme.Name));
          AuthenticationTicket ticket = new(principal, Scheme.Name);

          return AuthenticateResult.Success(ticket);
        }
        catch (Exception exception)
        {
          return AuthenticateResult.Fail(exception);
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
