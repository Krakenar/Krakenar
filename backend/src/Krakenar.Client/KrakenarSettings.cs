using Logitar;
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

    settings.BaseUrl = EnvironmentHelper.TryGetString("KRAKENAR_BASE_URL") ?? settings.BaseUrl;

    settings.ApiKey = EnvironmentHelper.TryGetString("KRAKENAR_API_KEY") ?? settings.ApiKey;

    string? username = EnvironmentHelper.TryGetString("KRAKENAR_BASIC_USERNAME") ?? settings.Basic?.Username.CleanTrim();
    string? password = EnvironmentHelper.TryGetString("KRAKENAR_BASIC_PASSWORD") ?? settings.Basic?.Password.CleanTrim();
    if (username is not null && password is not null)
    {
      settings.Basic = new BasicCredentials(username, password);
    }
    settings.Bearer = EnvironmentHelper.TryGetString("KRAKENAR_BEARER_TOKEN") ?? settings.Bearer;

    settings.Realm = EnvironmentHelper.TryGetString("KRAKENAR_REALM") ?? settings.Realm;
    settings.User = EnvironmentHelper.TryGetString("KRAKENAR_USER") ?? settings.User;

    return settings;
  }
}
