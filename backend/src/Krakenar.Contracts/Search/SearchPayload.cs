namespace Krakenar.Contracts.Search;

public record SearchPayload
{
  public List<Guid> Ids { get; set; } = [];
  public TextSearch Search { get; set; } = new();

  public List<SortOption> Sort { get; set; } = [];

  public int Skip { get; set; }
  public int Limit { get; set; }
}
