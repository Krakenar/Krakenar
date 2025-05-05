namespace Krakenar.Contracts.ApiKeys;

public record CreateOrReplaceApiKeyResult
{
  public ApiKey? ApiKey { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceApiKeyResult()
  {
  }

  public CreateOrReplaceApiKeyResult(ApiKey? apiKey, bool created)
  {
    ApiKey = apiKey;
    Created = created;
  }
}
