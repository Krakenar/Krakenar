using Krakenar.Contracts.Realms;

namespace Krakenar.Core.Tokens;

public static class TokenHelper
{
  public static string ResolveAudience(string? audience, Realm? realm, string baseUrl)
  {
    if (!string.IsNullOrWhiteSpace(audience))
    {
      return FormatAudienceOrIssuer(audience.Trim(), realm, baseUrl);
    }
    else if (realm != null)
    {
      return realm.Url ?? realm.UniqueSlug;
    }

    return baseUrl;
  }

  public static string ResolveIssuer(string? issuer, Realm? realm, string baseUrl)
  {
    if (!string.IsNullOrWhiteSpace(issuer))
    {
      return FormatAudienceOrIssuer(issuer.Trim(), realm, baseUrl);
    }
    else if (realm != null)
    {
      return FormatAudienceOrIssuer("{BaseUrl}/app/realms/{UniqueSlug}", realm, baseUrl);
    }

    return baseUrl;
  }

  private static string FormatAudienceOrIssuer(string format, Realm? realm, string baseUrl)
  {
    if (realm != null)
    {
      format = format.Replace("{Id}", realm.Id.ToString())
        .Replace("{UniqueSlug}", realm.UniqueSlug)
        .Replace("{Url}", realm.Url);
    }

    return format.Replace("{BaseUrl}", baseUrl);
  }
}
