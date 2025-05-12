using Krakenar.Contracts.Actors;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Caching;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Web;

public class HttpApplicationContext : IApplicationContext
{
  protected virtual ICacheService CacheService { get; }
  protected virtual IHttpContextAccessor HttpContextAccessor { get; }

  protected virtual Configuration Configuration => CacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    CacheService = cacheService;
    HttpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      User? user = HttpContextAccessor.HttpContext?.GetUser();
      if (user is not null)
      {
        return new Actor(user).GetActorId();
      }

      ApiKey? apiKey = HttpContextAccessor.HttpContext?.GetApiKey();
      if (apiKey is not null)
      {
        return new Actor(apiKey).GetActorId();
      }

      return null;
    }
  }

  public string BaseUrl
  {
    get
    {
      HttpContext context = HttpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(HttpContextAccessor.HttpContext)} is required.");
      return new Uri($"{context.Request.Scheme}://{context.Request.Host}", UriKind.Absolute).ToString().Trim('/');
    }
  }

  public RealmDto? Realm => HttpContextAccessor.HttpContext?.GetRealm();
  public RealmId? RealmId => Realm is null ? null : new RealmId(Realm.Id);

  public Secret Secret => new(Realm?.Secret ?? Configuration.Secret ?? string.Empty);
  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? Configuration.UniqueNameSettings;
  public IPasswordSettings PasswordSettings => Realm?.PasswordSettings ?? Configuration.PasswordSettings;
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? false;
  public bool RequireConfirmedAccount => Realm?.RequireConfirmedAccount ?? false;
}
