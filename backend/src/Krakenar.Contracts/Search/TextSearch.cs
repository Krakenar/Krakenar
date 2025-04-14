namespace Krakenar.Contracts.Search;

public record TextSearch
{
  public List<SearchTerm> Terms { get; set; } = [];
  public SearchOperator Operator { get; set; }

  public TextSearch(IEnumerable<SearchTerm>? terms = null, SearchOperator @operator = SearchOperator.And)
  {
    if (terms is not null)
    {
      Terms.AddRange(terms);
    }
    Operator = @operator;
  }
}
