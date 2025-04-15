﻿using Krakenar.Contracts.Configurations;
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
  protected virtual HttpContext Context => HttpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(HttpContextAccessor.HttpContext)} is required.");

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    CacheService = cacheService;
    HttpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      User? user = Context.GetUser();
      if (user is not null)
      {
        return user.GetActorId();
      }

      return null; // TODO(fpion): API key
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
  public IPasswordSettings PasswordSettings => Realm?.PasswordSettings ?? Configuration.PasswordSettings;
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? false;
  public bool RequireConfirmedAccount => Realm?.RequireConfirmedAccount ?? false;
}
