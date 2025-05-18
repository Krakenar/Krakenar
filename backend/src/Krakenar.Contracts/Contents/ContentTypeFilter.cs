namespace Krakenar.Contracts.Contents;

public record ContentTypeFilter
{
  public List<int> Ids { get; set; } = [];
  public List<Guid> Uids { get; set; } = [];
  public List<string> Names { get; set; } = [];
}
