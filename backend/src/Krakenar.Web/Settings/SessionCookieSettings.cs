namespace Krakenar.Web.Settings;

public record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
}
