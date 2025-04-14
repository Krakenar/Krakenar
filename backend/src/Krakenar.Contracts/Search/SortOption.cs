namespace Krakenar.Contracts.Search;

public record SortOption
{
  public string Field { get; set; }
  public bool IsDescending { get; set; }

  public SortOption() : this(string.Empty)
  {
  }

  public SortOption(string field, bool isDescending = false)
  {
    Field = field;
    IsDescending = isDescending;
  }
}
