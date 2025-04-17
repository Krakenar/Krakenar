using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Krakenar.Client;

public abstract class BaseClient : IDisposable
{
  protected virtual HttpClient HttpClient { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; } = new();

  protected BaseClient(IKrakenarSettings settings)
  {
    HttpClient = new();
    if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
    {
      HttpClient.BaseAddress = new Uri(settings.BaseUrl.Trim(), UriKind.Absolute);
    }
    // TODO(fpion): Authorization: Basic, Bearer
    // TODO(fpion): X-API-Key
    // TODO(fpion): X-Realm
    // TODO(fpion): X-User

    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public void Dispose()
  {
    HttpClient.Dispose();
    GC.SuppressFinalize(this);
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
    // TODO(fpion): Authorization: Basic, Bearer
    // TODO(fpion): X-API-Key
    // TODO(fpion): X-Realm
    // TODO(fpion): X-User

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
}
