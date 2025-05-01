namespace Krakenar.Contracts.Dictionaries;

public record CreateOrReplaceDictionaryResult
{
  public Dictionary? Dictionary { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceDictionaryResult()
  {
  }

  public CreateOrReplaceDictionaryResult(Dictionary? role, bool created)
  {
    Dictionary = role;
    Created = created;
  }
}
