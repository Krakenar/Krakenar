using Microsoft.Extensions.Configuration;

namespace Krakenar.Client;

public class KrakenarSettings : IKrakenarSettings
{
  public const string SectionKey = "Krakenar";

  public string? BaseUrl { get; set; }

  public BasicCredentials? Basic { get; set; }

  public string? Realm { get; set; }
  public string? User { get; set; }

  public static KrakenarSettings Initialize(IConfiguration configuration)
  {
    KrakenarSettings settings = configuration.GetSection(SectionKey).Get<KrakenarSettings>() ?? new();

    string? baseUrl = Environment.GetEnvironmentVariable("KRAKENAR_BASE_URL");
    if (!string.IsNullOrWhiteSpace(baseUrl))
    {
      settings.BaseUrl = baseUrl.Trim();
    }

    string? username = Environment.GetEnvironmentVariable("KRAKENAR_BASIC_USERNAME");
    string? password = Environment.GetEnvironmentVariable("KRAKENAR_BASIC_PASSWORD");
    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
    {
      settings.Basic = new BasicCredentials(username.Trim(), password.Trim());
    }

    string? realm = Environment.GetEnvironmentVariable("KRAKENAR_REALM");
    if (!string.IsNullOrWhiteSpace(realm))
    {
      settings.Realm = realm.Trim();
    }
    string? user = Environment.GetEnvironmentVariable("KRAKENAR_USER");
    if (!string.IsNullOrWhiteSpace(user))
    {
      settings.User = user.Trim();
    }

    return settings;
  }
}
