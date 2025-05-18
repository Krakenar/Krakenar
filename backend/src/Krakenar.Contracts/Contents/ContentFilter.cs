namespace Krakenar.Contracts.Contents;

public record ContentFilter
{
  public List<int> Ids { get; set; } = [];
  public List<Guid> Uids { get; set; } = [];
}
