namespace Krakenar.Web.Constants;

public static class Schemes
{
  public const string ApiKey = "ApiKey";
  public const string Basic = "Basic";
  public const string Bearer = "Bearer";
  public const string Session = "Session";

  public static string[] GetEnabled(IConfiguration configuration)
  {
    List<string> schemes = [ApiKey, Bearer, Session];
    if (configuration.GetValue<bool>("EnableBasicAuthentication"))
    {
      schemes.Add(Basic);
    }
    return [.. schemes];
  }
}
