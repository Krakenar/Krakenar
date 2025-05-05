namespace Krakenar.Contracts.ApiKeys;

public record AuthenticateApiKeyPayload
{
  public string XApiKey { get; set; } = string.Empty;

  public AuthenticateApiKeyPayload()
  {
  }

  public AuthenticateApiKeyPayload(string xApiKey)
  {
    XApiKey = xApiKey;
  }
}
