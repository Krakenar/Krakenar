using Krakenar.Contracts.Constants;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Tokens;
using Krakenar.Contracts.Users;
using Krakenar.Web.Models.Authentication;
using Krakenar.Web.Settings;
using Logitar.Security.Claims;
using Claim = System.Security.Claims.Claim;
using ClaimDto = Krakenar.Contracts.Tokens.Claim;

namespace Krakenar.Web.Authentication;

public interface IOpenAuthenticationService
{
  Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken = default);
  Task<TokenResponse> GetTokenResponseAsync(User user, CancellationToken cancellationToken = default);

  Task<User> GetUserAsync(string accessToken, CancellationToken cancellationToken = default);
}

public class OpenAuthenticationService : IOpenAuthenticationService
{
  protected virtual AuthenticationSettings Settings { get; }
  protected virtual AccessTokenSettings AccessTokenSettings => Settings.AccessToken;

  protected virtual ITokenService TokenService { get; }

  public OpenAuthenticationService(AuthenticationSettings settings, ITokenService tokenService)
  {
    Settings = settings;
    TokenService = tokenService;
  }

  public virtual async Task<TokenResponse> GetTokenResponseAsync(Session session, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = GetCreateTokenPayload(session.User, session);
    return await GetTokenResponseAsync(payload, session.RefreshToken, cancellationToken);
  }
  public virtual async Task<TokenResponse> GetTokenResponseAsync(User user, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = GetCreateTokenPayload(user);
    return await GetTokenResponseAsync(payload, refreshToken: null, cancellationToken);
  }
  protected virtual CreateTokenPayload GetCreateTokenPayload(User user, Session? session = null)
  {
    CreateTokenPayload payload = new()
    {
      LifetimeSeconds = AccessTokenSettings.LifetimeSeconds,
      Type = AccessTokenSettings.Type,
      Subject = user.Id.ToString()
    };

    DateTime? authenticationTime = user.AuthenticatedOn ?? session?.CreatedOn;
    if (authenticationTime.HasValue)
    {
      Claim claim = ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, authenticationTime.Value);
      payload.Claims.Add(new ClaimDto(claim.Type, claim.Value, claim.ValueType));
    }

    foreach (Role role in user.Roles)
    {
      payload.Claims.Add(new ClaimDto(Rfc7519ClaimNames.Roles, role.UniqueName));
    }

    if (session is not null)
    {
      payload.Claims.Add(new ClaimDto(Rfc7519ClaimNames.SessionId, session.Id.ToString()));
    }

    return payload;
  }
  protected virtual async Task<TokenResponse> GetTokenResponseAsync(CreateTokenPayload payload, string? refreshToken, CancellationToken cancellationToken)
  {
    CreatedToken access = await TokenService.CreateAsync(payload, cancellationToken);

    TokenResponse response = new(Schemes.Bearer, access.Token)
    {
      ExpiresIn = AccessTokenSettings.LifetimeSeconds,
      RefreshToken = refreshToken
    };
    return response;
  }

  public virtual async Task<User> GetUserAsync(string accessToken, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = new(accessToken)
    {
      Type = AccessTokenSettings.Type
    };
    ValidatedToken validatedToken = await TokenService.ValidateAsync(payload, cancellationToken);

    User user = new();

    if (!string.IsNullOrWhiteSpace(validatedToken.Subject))
    {
      user.Id = Guid.Parse(validatedToken.Subject);
    }

    Guid? sessionId = null;
    foreach (ClaimDto claim in validatedToken.Claims)
    {
      switch (claim.Name)
      {
        case Rfc7519ClaimNames.AuthenticationTime:
          Claim authenticationTime = new Claim(claim.Name, claim.Value, claim.Type);
          user.AuthenticatedOn = ClaimHelper.ExtractDateTime(authenticationTime);
          break;
        case Rfc7519ClaimNames.Roles:
          user.Roles.Add(new Role(claim.Value));
          break;
        case Rfc7519ClaimNames.SessionId:
          sessionId = Guid.Parse(claim.Value);
          break;
      }
    }
    if (sessionId.HasValue)
    {
      Session session = new(user)
      {
        Id = sessionId.Value,
        IsActive = true
      };
      if (user.AuthenticatedOn.HasValue)
      {
        session.CreatedOn = user.AuthenticatedOn.Value;
        session.UpdatedOn = user.AuthenticatedOn.Value;
      }
      user.Sessions.Add(session);
    }

    return user;
  }
}
