namespace Krakenar.Contracts.ApiKeys;

public record CreateOrReplaceApiKeyPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
  public List<string> Roles { get; set; } = [];

  public CreateOrReplaceApiKeyPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceApiKeyPayload(string name)
  {
    Name = name;
  }
}
