using Krakenar.Contracts.Constants;

namespace Krakenar.Client;

public abstract class BaseClient
{
  protected virtual HttpClient HttpClient { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; } = new();

  protected BaseClient(HttpClient httpClient, IKrakenarSettings settings)
  {
    HttpClient = httpClient;
    if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
    {
      HttpClient.BaseAddress = new Uri(settings.BaseUrl.Trim(), UriKind.Absolute);
    }
    if (settings.Basic is not null && !string.IsNullOrWhiteSpace(settings.Basic.Username) && !string.IsNullOrWhiteSpace(settings.Basic.Password))
    {
      string value = string.Join(' ', Schemes.Basic, Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.Basic.ToString())));
      HttpClient.DefaultRequestHeaders.Add(Headers.Authorization, value);
    }
    if (!string.IsNullOrWhiteSpace(settings.Bearer))
    {
      string value = string.Join(' ', Schemes.Bearer, settings.Bearer.Trim());
      HttpClient.DefaultRequestHeaders.Add(Headers.Authorization, value);
    }
    if (!string.IsNullOrWhiteSpace(settings.Realm))
    {
      HttpClient.DefaultRequestHeaders.Add(Headers.Realm, settings.Realm.Trim());
    }
    if (!string.IsNullOrWhiteSpace(settings.User))
    {
      HttpClient.DefaultRequestHeaders.Add(Headers.User, settings.User.Trim());
    }

    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  }

  protected virtual async Task<ApiResult<T>> DeleteAsync<T>(Uri uri, CancellationToken cancellationToken)
    => await SendAsync<T>(HttpMethod.Delete, uri, cancellationToken);

  protected virtual async Task<ApiResult<T>> GetAsync<T>(Uri uri, CancellationToken cancellationToken)
    => await SendAsync<T>(HttpMethod.Get, uri, cancellationToken);

  protected virtual async Task<ApiResult<T>> PatchAsync<T>(Uri uri, CancellationToken cancellationToken)
    => await PatchAsync<T>(uri, payload: null, cancellationToken);
  protected virtual async Task<ApiResult<T>> PatchAsync<T>(Uri uri, object? payload, CancellationToken cancellationToken)
    => await SendAsync<T>(HttpMethod.Patch, uri, payload, cancellationToken);

  protected virtual async Task<ApiResult<T>> PostAsync<T>(Uri uri, CancellationToken cancellationToken)
    => await PostAsync<T>(uri, payload: null, cancellationToken);
  protected virtual async Task<ApiResult<T>> PostAsync<T>(Uri uri, object? payload, CancellationToken cancellationToken)
    => await SendAsync<T>(HttpMethod.Post, uri, payload, cancellationToken);

  protected virtual async Task<ApiResult<T>> PutAsync<T>(Uri uri, CancellationToken cancellationToken)
    => await PutAsync<T>(uri, payload: null, cancellationToken);
  protected virtual async Task<ApiResult<T>> PutAsync<T>(Uri uri, object? payload, CancellationToken cancellationToken)
    => await SendAsync<T>(HttpMethod.Put, uri, payload, cancellationToken);

  protected virtual async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, Uri uri, CancellationToken cancellationToken)
   => await SendAsync<T>(method, uri, payload: null, cancellationToken);
  protected virtual async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, Uri uri, object? payload, CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(method, uri);
    if (payload is not null)
    {
      request.Content = JsonContent.Create(payload, payload.GetType(), mediaType: null, SerializerOptions);
    }

    using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);

    ApiResult<T> result = new()
    {
      StatusCode = response.StatusCode,
      IsSuccessStatusCode = response.IsSuccessStatusCode,
      ReasonPhrase = string.IsNullOrWhiteSpace(response.ReasonPhrase) ? null : response.ReasonPhrase.Trim(),
      Version = response.Version
    };

    try
    {
      string? content = await response.Content.ReadAsStringAsync(cancellationToken);
      if (!string.IsNullOrWhiteSpace(content))
      {
        result.Content = content.Trim();
      }
    }
    catch (Exception)
    {
    }

    Dictionary<string, IReadOnlyCollection<string>> headers = [];
    foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
    {
      headers[header.Key] = header.Value.ToList().AsReadOnly();
    }
    result.Headers = headers.AsReadOnly();

    Dictionary<string, IReadOnlyCollection<string>> trailingHeaders = [];
    foreach (KeyValuePair<string, IEnumerable<string>> trailingHeader in response.TrailingHeaders)
    {
      trailingHeaders[trailingHeader.Key] = trailingHeader.Value.ToList().AsReadOnly();
    }
    result.TrailingHeaders = trailingHeaders.AsReadOnly();

    if (!response.IsSuccessStatusCode)
    {
      ProblemDetails? problemDetails = null;
      if (result.Content is not null)
      {
        try
        {
          problemDetails = JsonSerializer.Deserialize<ProblemDetails>(result.Content, SerializerOptions);
        }
        catch (Exception)
        {
        }
      }

      if (response.StatusCode != HttpStatusCode.NotFound || (problemDetails is not null && problemDetails.Error is not null))
      {
        throw new KrakenarClientException(result, problemDetails);
      }
    }
    else if (result.Content is not null)
    {
      result.Value = JsonSerializer.Deserialize<T>(result.Content, SerializerOptions);
    }

    return result;
  }

  protected virtual InvalidApiResponseException CreateInvalidApiResponseException(string methodName, HttpMethod httpMethod, Uri uri, object? content = null)
  {
    string? serializedContent = content is null ? null : JsonSerializer.Serialize(content, content.GetType(), SerializerOptions);
    return new InvalidApiResponseException(GetType(), methodName, httpMethod, uri, serializedContent);
  }
}
