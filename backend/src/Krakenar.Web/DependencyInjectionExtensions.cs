﻿using Krakenar.Core;
using Krakenar.Web.Authentication;
using Krakenar.Web.Authorization;
using Krakenar.Web.Constants;
using Krakenar.Web.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Krakenar.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddControllersWithViews()
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    string[] authenticationSchemes = Schemes.GetEnabled(configuration);

    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(authenticationSchemes).RequireAuthenticatedUser().Build())
      .AddPolicy(Policies.KrakenarAdmin, new AuthorizationPolicyBuilder(authenticationSchemes)
        .RequireAuthenticatedUser()
        .AddRequirements(new KrakenarAdminRequirement())
        .Build());
    services.AddSingleton<IAuthorizationHandler, KrakenarAdminHandler>();

    CookiesSettings cookiesSettings = CookiesSettings.Initialize(configuration);
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    return services
      .AddExceptionHandler<ExceptionHandler>()
      .AddHttpContextAccessor()
      .AddProblemDetails()
      .AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
