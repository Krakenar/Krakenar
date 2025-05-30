namespace Krakenar.Client;

public record RequestContext
{
  public CancellationToken CancellationToken { get; set; }

  public string? ApiKey { get; set; }
  public BasicCredentials? Basic { get; set; }
  public string? Bearer { get; set; }

  public string? Realm { get; set; }
  public string? User { get; set; }

  public RequestContext(CancellationToken cancellationToken = default)
  {
    CancellationToken = cancellationToken;
  }
}
