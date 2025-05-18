namespace Krakenar.Contracts.Contents;

public record LanguageFilter
{
  public List<int> Ids { get; set; } = [];
  public List<Guid> Uids { get; set; } = [];
  public List<string> Codes { get; set; } = [];

  public bool? IsDefault { get; set; }
}
