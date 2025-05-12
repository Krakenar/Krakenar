using Microsoft.Extensions.Configuration;

namespace Krakenar.Client;

public interface IKrakenarSettings
{
  string? BaseUrl { get; }

  string? ApiKey { get; }
  BasicCredentials? Basic { get; }
  string? Bearer { get; }

  string? Realm { get; }
  string? User { get; }
}

public class KrakenarSettings : IKrakenarSettings
{
  public const string SectionKey = "Krakenar";

  public string? BaseUrl { get; set; }

  public string? ApiKey { set; get; }
  public BasicCredentials? Basic { get; set; }
  public string? Bearer { get; set; }

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

    string? apiKey = Environment.GetEnvironmentVariable("KRAKENAR_API_KEY");
    if (!string.IsNullOrWhiteSpace(apiKey))
    {
      settings.ApiKey = apiKey.Trim();
    }
    string? username = Environment.GetEnvironmentVariable("KRAKENAR_BASIC_USERNAME");
    string? password = Environment.GetEnvironmentVariable("KRAKENAR_BASIC_PASSWORD");
    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
    {
      settings.Basic = new BasicCredentials(username.Trim(), password.Trim());
    }
    string? bearer = Environment.GetEnvironmentVariable("KRAKENAR_BEARER_TOKEN");
    if (!string.IsNullOrWhiteSpace(bearer))
    {
      settings.Bearer = bearer.Trim();
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
