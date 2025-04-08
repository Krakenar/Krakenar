using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Caching;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Web;

public class HttpApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  private Configuration Configuration => _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      return null; // TODO(fpion): read from HttpContext
    }
  }
  public RealmDto? Realm
  {
    get
    {
      return null; // TODO(fpion): read from HttpContext
    }
  }
  public RealmId? RealmId => Realm is null ? null : new RealmId(Realm.Id);

  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? Configuration.UniqueNameSettings;
}
