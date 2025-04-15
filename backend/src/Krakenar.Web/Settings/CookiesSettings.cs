namespace Krakenar.Web.Settings;

public record CookiesSettings
{
  public const string SectionKey = "Cookies";

  public RefreshTokenCookieSettings RefreshToken { get; set; } = new();
  public SessionCookieSettings Session { get; set; } = new();

  public static CookiesSettings Initialize(IConfiguration configuration) => configuration.GetSection(SectionKey).Get<CookiesSettings>() ?? new();
}
