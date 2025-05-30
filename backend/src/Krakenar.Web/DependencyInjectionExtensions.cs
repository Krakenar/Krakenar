using Krakenar.Contracts.Constants;
using Krakenar.Core;
using Krakenar.Web.Authentication;
using Krakenar.Web.Authorization;
using Krakenar.Web.Constants;
using Krakenar.Web.Filters;
using Krakenar.Web.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Krakenar.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarWeb(this IServiceCollection services, IConfiguration configuration)
  {
    AdminSettings adminSettings = AdminSettings.Initialize(configuration);
    services.AddSingleton(adminSettings);
    services.AddControllersWithViews(options => options.Filters.Add<Logging>())
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    services.AddSingleton(CorsSettings.Initialize(configuration));
    services.AddCors();

    AuthenticationSettings authenticationSettings = AuthenticationSettings.Initialize(configuration);
    services.AddSingleton(authenticationSettings);
    string[] authenticationSchemes = authenticationSettings.GetKrakenarAuthenticationSchemes();
    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (authenticationSettings.EnableBasic)
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }
    services.AddTransient<IOpenAuthenticationService, OpenAuthenticationService>();

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

    ErrorSettings settings = ErrorSettings.Initialize(configuration);
    services.AddSingleton(settings);

    return services
      .AddExceptionHandler<ExceptionHandler>()
      .AddHttpContextAccessor()
      .AddProblemDetails()
      .AddSingleton<IApplicationContext, HttpApplicationContext>();
  }

  public static string[] GetKrakenarAuthenticationSchemes(this AuthenticationSettings settings)
  {
    List<string> schemes = new(capacity: 4)
    {
      Schemes.ApiKey,
      Schemes.Bearer,
      Schemes.Session
    };

    if (settings.EnableBasic)
    {
      schemes.Add(Schemes.Basic);
    }

    return [.. schemes];
  }
}
