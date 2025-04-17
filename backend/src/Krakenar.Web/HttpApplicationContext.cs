using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Caching;
using Krakenar.Core.Realms;
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

      // ISSUE #15: https://github.com/Krakenar/Krakenar/issues/15

      return null;
    }
  }
  public RealmDto? Realm => HttpContextAccessor.HttpContext?.GetRealm();
  public RealmId? RealmId => Realm is null ? null : new RealmId(Realm.Id);

  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? Configuration.UniqueNameSettings;
  public IPasswordSettings PasswordSettings => Realm?.PasswordSettings ?? Configuration.PasswordSettings;
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? false;
  public bool RequireConfirmedAccount => Realm?.RequireConfirmedAccount ?? false;
}
