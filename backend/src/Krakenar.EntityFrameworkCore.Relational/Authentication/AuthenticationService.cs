using Krakenar.Core.ApiKeys;
using Krakenar.Core.Authentication;
using Krakenar.Core.Users;
using Krakenar.EntityFrameworkCore.Relational.Settings;
using Logitar;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational.Authentication;

public class AuthenticationService : IAuthenticationService
{
  protected virtual IApiKeyRepository ApiKeyRepository { get; }
  protected virtual KrakenarContext Context { get; }
  protected virtual AuthenticationSettings Settings { get; }
  protected virtual IUserRepository UserRepository { get; }

  public AuthenticationService(IApiKeyRepository apiKeyRepository, KrakenarContext context, AuthenticationSettings settings, IUserRepository userRepository)
  {
    ApiKeyRepository = apiKeyRepository;
    Context = context;
    Settings = settings;
    UserRepository = userRepository;
  }

  public virtual async Task AuthenticatedAsync(ApiKey apiKey, CancellationToken cancellationToken)
  {
    if (Settings.EnableAuthenticatedEventSourcing)
    {
      await ApiKeyRepository.SaveAsync(apiKey, cancellationToken);
    }
    else if (apiKey.AuthenticatedOn.HasValue)
    {
      await Context.ApiKeys
        .Where(x => x.StreamId == apiKey.Id.Value)
        .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.AuthenticatedOn, apiKey.AuthenticatedOn.Value.AsUniversalTime()), cancellationToken);
    }
  }

  public virtual async Task AuthenticatedAsync(User user, CancellationToken cancellationToken)
  {
    if (Settings.EnableAuthenticatedEventSourcing)
    {
      await UserRepository.SaveAsync(user, cancellationToken);
    }
    else if (user.AuthenticatedOn.HasValue)
    {
      await Context.Users
        .Where(x => x.StreamId == user.Id.Value)
        .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.AuthenticatedOn, user.AuthenticatedOn.Value.AsUniversalTime()), cancellationToken);
    }
  }
}
