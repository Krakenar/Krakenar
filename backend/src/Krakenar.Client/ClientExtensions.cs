using Krakenar.Contracts.Constants;

namespace Krakenar.Client;

public static class ClientExtensions
{
  public static void ApplyContext(this HttpRequestMessage request, RequestContext context)
  {
    if (!string.IsNullOrWhiteSpace(context.ApiKey))
    {
      request.Headers.Add(Headers.ApiKey, context.ApiKey);
    }
    if (context.Basic is not null && !string.IsNullOrWhiteSpace(context.Basic.Username) && !string.IsNullOrWhiteSpace(context.Basic.Password))
    {
      string value = string.Join(' ', Schemes.Basic, Convert.ToBase64String(Encoding.UTF8.GetBytes(context.Basic.ToString())));
      request.Headers.Add(Headers.Authorization, value);
    }
    if (!string.IsNullOrWhiteSpace(context.Bearer))
    {
      string value = string.Join(' ', Schemes.Bearer, context.Bearer.Trim());
      request.Headers.Add(Headers.Authorization, value);
    }
    if (!string.IsNullOrWhiteSpace(context.Realm))
    {
      request.Headers.Add(Headers.Realm, context.Realm.Trim());
    }
    if (!string.IsNullOrWhiteSpace(context.User))
    {
      request.Headers.Add(Headers.User, context.User.Trim());
    }
  }

  public static void ApplySettings(this HttpClient client, IKrakenarSettings settings)
  {
    if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
    {
      client.BaseAddress = new Uri(settings.BaseUrl.Trim(), UriKind.Absolute);
    }
    if (!string.IsNullOrWhiteSpace(settings.ApiKey))
    {
      client.DefaultRequestHeaders.Add(Headers.ApiKey, settings.ApiKey);
    }
    if (settings.Basic is not null && !string.IsNullOrWhiteSpace(settings.Basic.Username) && !string.IsNullOrWhiteSpace(settings.Basic.Password))
    {
      string value = string.Join(' ', Schemes.Basic, Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.Basic.ToString())));
      client.DefaultRequestHeaders.Add(Headers.Authorization, value);
    }
    if (!string.IsNullOrWhiteSpace(settings.Bearer))
    {
      string value = string.Join(' ', Schemes.Bearer, settings.Bearer.Trim());
      client.DefaultRequestHeaders.Add(Headers.Authorization, value);
    }
    if (!string.IsNullOrWhiteSpace(settings.Realm))
    {
      client.DefaultRequestHeaders.Add(Headers.Realm, settings.Realm.Trim());
    }
    if (!string.IsNullOrWhiteSpace(settings.User))
    {
      client.DefaultRequestHeaders.Add(Headers.User, settings.User.Trim());
    }
  }
}
