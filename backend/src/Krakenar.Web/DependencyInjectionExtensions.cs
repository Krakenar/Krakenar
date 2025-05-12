using Krakenar.Contracts.Constants;
using Krakenar.Core;
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

    services.AddSingleton(CorsSettings.Initialize(configuration));
    services.AddCors();

    string[] authenticationSchemes = configuration.GetKrakenarAuthenticationSchemes();
    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }

    OpenAuthenticationSettings openAuthenticationSettings = OpenAuthenticationSettings.Initialize(configuration);
    services.AddSingleton(openAuthenticationSettings);
    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(authenticationSchemes).RequireAuthenticatedUser().Build())
      .AddPolicy(Policies.KrakenarAdmin, new AuthorizationPolicyBuilder(authenticationSchemes)
        .RequireAuthenticatedUser()
        .AddRequirements(new KrakenarAdminRequirement())
        .Build());
    services.AddSingleton<IAuthorizationHandler, KrakenarAdminHandler>();
    services.AddTransient<IOpenAuthenticationService, OpenAuthenticationService>();

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

  public static string[] GetKrakenarAuthenticationSchemes(this IConfiguration configuration)
  {
    List<string> schemes = new(capacity: 4)
    {
      Schemes.ApiKey,
      Schemes.Bearer,
      Schemes.Session
    };

    string? enableBasicAuthentication = Environment.GetEnvironmentVariable("ENABLE_BASIC_AUTHENTICATION");
    if (string.IsNullOrWhiteSpace(enableBasicAuthentication) || !bool.TryParse(enableBasicAuthentication, out bool isBasicAuthenticationEnabled))
    {
      isBasicAuthenticationEnabled = configuration.GetValue<bool>("EnableBasicAuthentication");
    }
    if (isBasicAuthenticationEnabled)
    {
      schemes.Add(Schemes.Basic);
    }

    return [.. schemes];
  }
}
