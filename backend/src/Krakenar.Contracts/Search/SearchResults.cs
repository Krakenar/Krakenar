namespace Krakenar.Contracts.Search;

public record SearchResults<T>
{
  public List<T> Items { get; set; } = [];
  public long Total { get; set; }

  public SearchResults()
  {
  }

  public SearchResults(long total)
  {
    Total = total;
  }

  public SearchResults(IEnumerable<T> items) : this(items, items.LongCount())
  {
  }

  public SearchResults(IEnumerable<T> items, long total)
  {
    Items.AddRange(items);
    Total = total;
  }
}
