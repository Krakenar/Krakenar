using System.Net;

namespace Krakenar.Client;

public record ApiResult
{
  public HttpStatusCode StatusCode { get; set; }
  public bool IsSuccessStatusCode { get; set; }
  public string? ReasonPhrase { get; set; }
  public Version Version { get; set; }

  public string? Content { get; set; }

  public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers { get; set; }
  public IReadOnlyDictionary<string, IReadOnlyCollection<string>> TrailingHeaders { get; set; }

  public ApiResult()
  {
    Version = new Version();

    Headers = new Dictionary<string, IReadOnlyCollection<string>>().AsReadOnly();
    TrailingHeaders = new Dictionary<string, IReadOnlyCollection<string>>().AsReadOnly();
  }
}

public record ApiResult<T> : ApiResult
{
  public T? Value { get; set; }

  public ApiResult() : base()
  {
  }
}
